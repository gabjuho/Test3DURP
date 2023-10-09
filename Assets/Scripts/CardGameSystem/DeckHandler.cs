using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckHandler : MonoBehaviour,IPointerClickHandler
{
    private DeckController _deckController;
    private DeckGroupController _deckGroupController;

    private void Awake()
    {
        _deckGroupController = GameObject.Find("PlayerDeckGroup").GetComponent<DeckGroupController>();
        _deckController = GetComponent<DeckController>();
    }
    
    public DeckHandler(DeckController controller)
    {
        _deckController = controller;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        _deckController.DrawCardFromDeck(false);
        _deckGroupController.InActivateAllDecks();
    }
}