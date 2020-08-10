using System.Linq;
using Assets.Scripts;
using Mediator;
using Model.Board;
using Model.Characters;
using Model.Characters.Controllers;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using View;

public class Arena : MonoBehaviour
{
	[SerializeField]
	private SquareBoardGenerator _squareBoardGenerator = default;
	
	[SerializeField] 
	private SquareGridBoardView _boardView = default;

	[SerializeField]
	private HUD _hud = default;
	
	private readonly ReactiveCollection<Character> _activeCharacters = new ReactiveCollection<Character>();
	private readonly ReactiveCollection<Character> _turnQueue = new ReactiveCollection<Character>();
	
	private readonly PlayerController _playerController = new PlayerController();
	private readonly ZombieController _zombieController = new ZombieController();
	
	private Character _currentCharacter;
	private IBoard _board;
	private CompositeDisposable _arenaDisposables;
	
	private void Start()
	{
		_arenaDisposables = new CompositeDisposable();

		var squareGridBoard = _squareBoardGenerator.GenerateBoard(_playerController, _zombieController);
		_board = squareGridBoard;
		
		_boardView.GenerateView(squareGridBoard);
		_boardView.AddTo(_arenaDisposables);

		foreach (var field in _board.AllFields)
			new FieldViewModelMediator(field, _boardView).AddTo(_arenaDisposables);

		new PlayerControllerViewModelMediator(_playerController, _boardView, _hud).AddTo(_arenaDisposables);
		new ActiveCharactersViewModelMediator(this, _boardView).AddTo(_arenaDisposables);

		foreach (var field in _board.AllFields)
		{
			if (field.OccupiedBy != null)
				AddCharacter(field.OccupiedBy);
		}
		
		StartNextTurn();
	}

	[ShowInInspector]
	private void Restart()
	{
		if (_arenaDisposables == null)
			return;
		
		_activeCharacters.Clear();
		_turnQueue.Clear();
		
		_arenaDisposables.Dispose();
		
		Debug.Log("Game restarted");
		
		Start();
	}
	
	private void AddCharacter(Character character)
	{
		_turnQueue.Add(character);
		_activeCharacters.Add(character);
		character.OnDispose += OnCharacterDisposed;
	}

	private void OnCharacterDisposed(Character character)
	{
		character.OnDispose -= OnCharacterDisposed;
		_turnQueue.Remove(character);
		_activeCharacters.Remove(character);
	}

	public void StartNextTurn()
	{
		if (_currentCharacter != null)
		{
			Debug.Log($"Turn ended - {_currentCharacter.ID}");
			_currentCharacter?.EndTurn();
		}

		if(!_turnQueue.Any())
			return;
		
		_currentCharacter = _turnQueue[0];
		
		if(_turnQueue.All(c => c.Team == 1))
		{
			Debug.Log("You lost");
			return;
		}
		
		if(_turnQueue.All(c => c.Team == 0))
		{
			Debug.Log("You won");
			return;
		}
		
		Debug.Log($"Turn started - {_currentCharacter.ID}");
		
		_turnQueue.RemoveAt(0);
		_turnQueue.Add(_currentCharacter);
		_currentCharacter.StartTurn();
		_currentCharacter.Controller.StartTurn(_currentCharacter, this);
	}

	public IReadOnlyReactiveCollection<Character> TurnQueue => _turnQueue;
	public IReadOnlyReactiveCollection<Character> ActiveCharacters => _activeCharacters;
	public IBoard Board => _board;
}