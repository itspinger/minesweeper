using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Game : MonoBehaviour
{
    private int _height;
    private int _width;
    private int _mines;
    
    private Field[,] _fields;
    private TileManager _tileManager;

    private bool _clicked = false;
    private bool _gameOver = false;
    
    // The time the game has started
    private readonly Stopwatch _stopwatch = new Stopwatch();

    private void Awake()
    {
        _tileManager = GetComponentInChildren<TileManager>();
        Application.targetFrameRate = 60;
    }

    public void Start()
    {
        // Load all the necessary game data
        _width = 9;
        _height = 9;
        _mines = 10;
        
        Camera.main.transform.position = new Vector3(_width / 2, _height / 2f, -10);
        Camera.main.orthographicSize = Math.Max(_height, _width) / 2f * 1.43f;

        _fields = new Field[_width, _height];
        FieldGenerator.CreateDefaultFieldTable(_fields, _tileManager);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }
    }

    private Field GetFieldFromMouse()
    {
        var world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mouse = _tileManager.GetTilemap().WorldToCell(world);

        if (mouse.x < 0 || mouse.x >= _width || mouse.y < 0 || mouse.y >= _height)
        {
            return null;
        }

        return _fields[mouse.x, mouse.y];
    }

    private void HandleLeftClick()
    {
        var field = GetFieldFromMouse();

        // This means that the field is invalid
        if (field == null)
            return;

        if (!_clicked)
        {
            // We generate mines after first click
            // Because we don't want the chance of the player
            // Losing within the first click
            new FieldGenerator(_fields, _mines, field);
            _tileManager.UpdateFields(_fields);

            // Update the click
            _clicked = true;
        }

        // This event has already been called for this field
        if (field.GetState() == Field.FieldState.Flagged || field.GetState() == Field.FieldState.Revealed)
            return;

        if (field.IsMine())
        {
            EndGame(field);
            return;
        }

        
        if (field.GetAdjacentMines() != 0)
        {
            field.SetState(Field.FieldState.Revealed);
            _tileManager.UpdateField(field);
            
            // Check the winning condition
            return;
        }

        FloodFill(field);
        field.SetState(Field.FieldState.Revealed);
        _tileManager.UpdateField(field);
    }

    private void FloodFill(Field field)
    {
        if (field.GetState() == Field.FieldState.Revealed)
            return;

        if (field.IsMine())
            return;

        field.SetState(Field.FieldState.Revealed);
        var pos = field.GetPosition();
        
        // Check if field has 0 adjacent; if so, flood again to 4 corners
        if (field.GetAdjacentMines() != 0)
            return;

        foreach (var adjacentField in GetAdjacentFields(field))
        {
            FloodFill(adjacentField);
        }
    }

    private IEnumerable<Field> GetAdjacentFields(Field field)
    {
        var adjacent = new List<Field>();
        var pos = field.GetPosition();

        for (var i = Math.Max(0, pos.x - 1); i <= Math.Min(_width - 1, pos.x + 1); i++)
        { 
            if (i == 0)
                continue;
            
            adjacent.Add(_fields[pos.x + i, pos.y]);
        }

        for (var i = Math.Max(0, pos.y - 1); i <= Math.Min(_height - 1, pos.y + 1); i++)
        {
            if (i == 0)
                continue;
            
            adjacent.Add(_fields[pos.x, pos.y + i]);
        }

        return adjacent;
    }

    private void EndGame(Field field)
    {
        if (!field.IsMine())
        {
            return;
        }
        
        Debug.Log("Game has finished");
        _gameOver = true;
        
        // Revealed state
        field.SetState(Field.FieldState.Revealed);
        field.SetExploded(true);
        
        // Reveal all others
        foreach (var f in _fields)
        {
            f.SetState(Field.FieldState.Revealed);
        }
        
        _tileManager.UpdateFields(_fields);
    }
    
    private void HandleRightClick()
    {
        var field = GetFieldFromMouse();

        // This means that the field is invalid
        if (field == null)
            return;
        
        // These states are not updated within the right click
        if (field.GetState() == Field.FieldState.Revealed || field.GetState() == Field.FieldState.Unknown)
            return;
        
        field.SetState(field.GetState() == Field.FieldState.Flagged ? Field.FieldState.Hidden : Field.FieldState.Flagged);
        _tileManager.UpdateField(field);
    }
    
}
