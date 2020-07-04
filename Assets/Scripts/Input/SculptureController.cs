using UnityEngine;

public class SculptureController : MonoBehaviour
{
    private SculpturePiece[] _pieces;
    private Transform[] _pieceTransforms;
    private Vector3[] _pieceRotations;

    [SerializeField]
    private LobbyDoorController _lobbyDoorController;

    [Header("Sculpture Piece Settings")] 
    [SerializeField]
    private float _defaultGlowIntensity = 1;
    
    [SerializeField]
    private float _interactingGlowIntensity = 2,
        _defaultGlowScaleMultiplier = 2f,
        _interactingGlowScaleMultiplier = 4f,
        _rotationSpeed = 0.5f;

    [SerializeField] 
    private Transform[] _nonInteractableOrbitingPieces;
    private Vector3[] _orbitPositions, _orbitRotations;
    private float _orbitSpeed;

    private void Awake()
    {
        _pieces = GetComponentsInChildren<SculpturePiece>();
        _pieceTransforms = new Transform[_pieces.Length];
        _pieceRotations = new Vector3[_pieces.Length];
        
        _orbitPositions = new Vector3[_nonInteractableOrbitingPieces.Length];
        _orbitRotations = new Vector3[_nonInteractableOrbitingPieces.Length];
        for (int i = 0; i < _orbitPositions.Length; i++)
        {
            _orbitPositions[i] = transform.position;
            _orbitPositions[i].y = _nonInteractableOrbitingPieces[i].position.y;
            _orbitRotations[i] = RandomRotation();
        }

        _orbitSpeed = _rotationSpeed / 2f;
    }

    private void Start()
    {
        for (int i = 0; i < _pieces.Length; i++)
        {
            _pieceTransforms[i] = _pieces[i].transform;
            _pieceRotations[i] = RandomRotation();
            
            SculpturePiece piece = _pieces[i];
            piece.Setup(_lobbyDoorController,
                _defaultGlowIntensity,
                _interactingGlowIntensity,
                _defaultGlowScaleMultiplier,
                _interactingGlowScaleMultiplier);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _pieceTransforms.Length; i++)
        {
            Transform piece = _pieceTransforms[i];
            piece.Rotate(_pieceRotations[i]);
        }
        
        for (int i = 0; i < _orbitPositions.Length; i++)
        { 
            _nonInteractableOrbitingPieces[i].RotateAround(_orbitPositions[i], Vector3.up, _orbitSpeed);
            _nonInteractableOrbitingPieces[i].Rotate(_orbitRotations[i]);
        }
    }
    
    private Vector3 RandomRotation()
    {
        float x = Random.Range(-_rotationSpeed, _rotationSpeed);
        float y = Random.Range(-_rotationSpeed, _rotationSpeed);
        float z = Random.Range(-_rotationSpeed, _rotationSpeed);
        return new Vector3(x, y, z);
    }
}
