using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed { get; private set; } = 5f;

    private PlayerMovementController movementController;

    [field: SerializeField] public PlayerPersistentState PersistentState { get; private set; }

    [SerializeField] private AudioChannel _channel;

    [field: SerializeField] public float TauntRadius { get; private set; } = 4f;

    [field: SerializeField] public GameObject TauntIndicator { get; private set; }

    private void Awake()
    {
        GetComponent<Animator>().runtimeAnimatorController = PersistentState.CharacterChoice.animations;
    }

    public void Start()
    {
        movementController = new PlayerMovementController(this, _channel);
        GameController.Instance.OnGameOver += onGameOver;
    }

    public void FixedUpdate()
    {
        movementController.OnUpdate();
    }

    private void onGameOver()
    {
        _channel.PlayAudio("Splat");
        Destroy(this.gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Stomper"))
        {
            GameController.Instance.GameOver();
        }
    }
}