using UnityEngine.UI;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance = null;


    public GameObject WSEquipWindow;
    public GameObject coreEquipWindow;
    public Button InitialButton;

    public Button[] EqItem = new Button[7];
    public Button[] EqMagic = new Button[7];

    public Button[] EqWeapon = new Button[2];
    public Button[] EqShield = new Button[2];
    public Button EqCore;

    /*    public Button Item0;
        public Button Item1;
        public Button Item2;
        public Button Item3;
        public Button Item4;
        public Button Item5;
        public Button Item6;
    */
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
