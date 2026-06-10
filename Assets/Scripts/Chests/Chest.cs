using DefaultNamespace.Combat.Items;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isPlayerNearby = false;
    public int chestId;
    public int numberOfItems = 1;
    [SerializeField] private BaseCombatItem potion;
    [SerializeField] private Sprite openedChestSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;


    void Start()
    {
        if (ChestProgressManager.Instance.openedChests[chestId])
        {
            spriteRenderer.sprite = openedChestSprite;
            numberOfItems = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerNearby && numberOfItems > 0)
        {
            PartyManager.Instance.Inventory.Add((BaseCombatItem)potion.CreateInstance());
            NarrationManager.Instance.ShowText(potion.Name + " added to inventory");
            spriteRenderer.sprite = openedChestSprite;
            numberOfItems--;
            ChestProgressManager.Instance.openedChests[chestId] = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && numberOfItems > 0)
        {
            isPlayerNearby = true;
            NarrationManager.Instance.ShowText("E - Open Chest");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = false;
            NarrationManager.Instance.HideText();
        }
    }
}
