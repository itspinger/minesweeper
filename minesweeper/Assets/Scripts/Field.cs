using UnityEngine;


/**
 * This class represents a field within the game table. Every
 * field has a set FieldType which determines whether the field
 * is a mine or not.
 *
 * Written by Dimitrije Mijailovic
 */

public class Field
{
	private readonly FieldType _type;
	private FieldState _state = FieldState.Hidden;

	private Vector3Int _position;
	private readonly int _adjacentMines;
	private bool _exploded;

	public Field(FieldType type, int adjacentMines)
	{
		_type = type;
		_adjacentMines = adjacentMines;
	}

	/*
	 * This method returns whether this field
	 * is a mine.
	 */
	
	public bool IsMine()
	{
		return _type == FieldType.Mine;
	}

	/**
	 * This method changes the current state of this field
	 * to the one specified in the arguments.
	 */
	
	public void SetState(FieldState state)
	{
		_state = state;
	}

	/**
	 * This method returns whether this specific field
	 * was the one that exploded, if any exploded at all.
	 */
	
	public bool HasExploded()
	{
		return _exploded;
	}

	/**
	 * This method returns the amount of mines adjacent
	 * to this field.
	 */

	public int GetAdjacentMines()
	{
		return _adjacentMines;
	}
	
	/*
	 * This method returns the field type of this field.
	 * 
	 * If the type is a mine, upon clicking on the field
	 * the game will be forced to end.
	 */

	public FieldType GetFieldType()
	{
		return _type;
	}

	/*
	 * This method returns the state of this field.
	 */
	
	public FieldState GetState()
	{
		return _state;
	}

	/**
	 * This method returns the position of this field.
	 */
	
	public Vector3Int GetPosition()
	{
		return _position;
	}

	/**
	 * This enum is used to represent the field type.
	 *
	 * There are only 2 allowed states of any field: Default, which corresponds
	 * to the field which isn't a mine, and a Mine field.
	 */

	public enum FieldType
	{
		Default,
		Mine,
		Unknown
	}
	
	/**
	 * This enum represents the current state of this field,
	 * which is changed when the field is clicked, or when the field
	 * is being flooded as the reason of clicking another field.
	 *
	 * The default state for each field is Hidden.
	 */

	public enum FieldState
	{
		Revealed,
		Hidden,
		Flagged,
		Unknown
	}
}
