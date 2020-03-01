using UnityEngine;
using Interactive.Base;
using System.Collections.Generic;

public class House : ChoicesSenderBehaviour
{
    [SerializeField]
    protected int currentHealth;
    [SerializeField]
    protected int maxHealth;

    public HealthBar healthBar;
    
    [SerializeField]
    private int repairingAmount;
    [SerializeField]
    private int reparationCosts;

    [SerializeField] private Player ownerPlayer;

    [SerializeField] private Player enemyPlayer;

    [SerializeField] private EndScreen endScreen;

    [SerializeField]
    private AudioSource themeAudio;

    [SerializeField]
    private AudioSource endGameAudio;

    [SerializeField]
    private AudioSource repairHouseAudio;

    public enum HouseState
    {
        FullHeatlh,
        LightlyDamaged,
        HeavilyDamaged,
        Destroyed
    }

    private HouseState currentState;

    private Timer timer;

    public HouseState CurrentState
    {
        get { return currentState;}
        set
        {
            currentState = value;
            UpdateSprite();
        }
    }

    [Header("From Destroyed (0) to Full Health (4)")]
    [SerializeField] 
    private List<Sprite> sprites;

    private Dictionary<HouseState, Sprite> spritesDictionnary;


    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            float newPercentage = (float)value / (float)maxHealth;

            UpdateOwnerMood(newPercentage);
            UpdateHealthBar(newPercentage);
            UpdateState(newPercentage);

            currentHealth = value;

            if (currentHealth <= 0)
            {
                themeAudio.Stop();
                endGameAudio.Play();
                endScreen.Show(enemyPlayer.PlayerId);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        //TODO Règler ce truc moche, commentez si besoin de tester d'autres valeurs
        CurrentHealth = maxHealth;

        //Init Dictionnary
        spritesDictionnary = new Dictionary<HouseState, Sprite>();

        spritesDictionnary.Add(HouseState.Destroyed,sprites[0]);
        spritesDictionnary.Add(HouseState.HeavilyDamaged, sprites[1]);
        spritesDictionnary.Add(HouseState.LightlyDamaged, sprites[2]);
        spritesDictionnary.Add(HouseState.FullHeatlh, sprites[3]);
    }

    private void UpdateHealthBar(float newPercentage)
    {
        if (healthBar != null)
        {
            healthBar.UpdateBar(newPercentage);
        }
    }

    private void UpdateOwnerMood(float newPercentage)
    {
        if (ownerPlayer != null)
        {
            ownerPlayer.ChangeMood(newPercentage);
        }
    }

    private void UpdateState(float newPercentage)
    {
        if (currentState != HouseState.Destroyed && newPercentage <= Mathf.Epsilon)
        {
            CurrentState = HouseState.Destroyed;
            return;
        }

        if (currentState != HouseState.HeavilyDamaged && newPercentage > Mathf.Epsilon && newPercentage <= 0.3f)
        {
            CurrentState = HouseState.HeavilyDamaged;
            return;
        }

        if (currentState != HouseState.LightlyDamaged && newPercentage > 0.3 && newPercentage <= 0.75)
        {
            CurrentState = HouseState.LightlyDamaged;
            return;
        }

        if (currentState != HouseState.FullHeatlh && newPercentage > 0.75)
        {
            CurrentState = HouseState.FullHeatlh;
            return;
        }
    }

    private void UpdateSprite()
    {
        this.GetComponent<SpriteRenderer>().sprite = spritesDictionnary[currentState];
    }

    public bool DoDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    public void Repair(Player player, int repairPoint)
    {
        repairHouseAudio.Play();
        CurrentHealth += repairPoint;
        if (CurrentHealth > maxHealth)
        {
            CurrentHealth = maxHealth;
        }
        Debug.Log("Hp maison: " + currentHealth);
        player.money -= reparationCosts;
    }

    public void RegisterTimer(Timer timer)
    {
        this.timer = timer;
    }

    public void EndGame()
    {
        if (timer != null)
        {
            timer.PauseTimer();
        }
        themeAudio.Stop();
        endGameAudio.Play();
        endScreen.Show(enemyPlayer.PlayerId);
    }

    public UserAction GetAction(Player player)
    {
        if (CurrentHealth == maxHealth || player.money < reparationCosts)
        {
            return null;
        } else
        {
            return new ComboAction(player.inputManager ,new List<string> { "←", "→" }, 2, () => Repair(player, repairingAmount), "Repair");
        }
    }

    public override List<GameAction> GetChoices(Player contextPlayer)
    {
        // Test
        bool lol = true;
        return new List<GameAction>() {
                new GameAction("Toquer", () => Debug.Log("Knock! Knock!"), () => true),
                new GameAction("Désactiver ce bouton", () => lol = false, () => lol),
            };
    }
}