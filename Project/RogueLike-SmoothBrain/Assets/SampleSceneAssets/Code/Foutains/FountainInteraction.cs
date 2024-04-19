using UnityEngine;

namespace Fountain
{
    [RequireComponent(typeof(FountainDisplay), typeof(Fountain))]
    public class FountainInteraction : MonoBehaviour, IInterractable
    {
        private Fountain fountain;
        private FountainDisplay display;
        private Hero hero;
        private PlayerInteractions interactions;
        private Outline outline;

        private bool isSelect = false;

        private void Start()
        {
            fountain = GetComponent<Fountain>();
            display = GetComponent<FountainDisplay>();
            hero = FindObjectOfType<Hero>();
            interactions = hero.GetComponent<PlayerInteractions>();
            outline = GetComponent<Outline>();
        }

        private void Update()
        {
            DetectInterctable();
        }

        private void DetectInterctable()
        {
            bool isInRange = Vector2.Distance(interactions.transform.position.ToCameraOrientedVec2(), transform.position.ToCameraOrientedVec2()) <= hero.Stats.GetValue(Stat.CATCH_RADIUS);

            if (isInRange && !interactions.InteractablesInRange.Contains(this))
            {
                interactions.InteractablesInRange.Add(this);
            }
            else if (!isInRange && interactions.InteractablesInRange.Contains(this))
            {
                interactions.InteractablesInRange.Remove(this);
                Deselect();
            }
        }

        public void Deselect()
        {
            if (!isSelect)
                return;

            isSelect = false;
            display.Undisplay();
            outline.DisableOutline();
        }

        public void Select()
        {
            if (isSelect)
                return;

            isSelect = true;
            display.Display();
            outline.EnableOutline();
        }

        public void Interract()
        {
            int price = fountain.BloodPrice;
            int trade = fountain.ValueTrade;

            if (price > hero.Inventory.Blood)
                return;

            hero.Inventory.Blood -= price;
            hero.Stats.IncreaseValue(Stat.CORRUPTION, trade);
            FloatingTextGenerator.CreateEffectDamageText(fountain.AbsoluteValueTrade, transform.position, fountain.Color);
        }
    }
}