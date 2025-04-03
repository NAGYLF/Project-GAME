using Items;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool IsPlayerBullet = false;

    [Header("Ballisztika paraméterek")]
    public float mass = 0.02f;               // kg
    public float dragCoefficient = 0.45f;    // C_d
    public float crossSectionArea = 0.001f;  // m^2
    public float airDensity = 1.225f;        // kg/m^3 (kb. tengerszinten)
    public float gravity = 9.81f;            // m/s^2 (Földön)

    public float Dmg;
    public float APPower;
    public float Caliber;

    [Header("Kezdeti feltételek")]
    public Vector3 initialVelocity; // A lövedék indulósebessége

    private Vector3 velocity;       // Aktuális sebesség
    private float k;                // Egyszerűsített drag-együttható: 0.5 * ρ * C_d * A
    private float simulatedHeight = 0f; // Magasság szimulációhoz

    public void Initialize(Ammunition ammunition, Vector3 fireDirection)
    {
        mass = ammunition.Mass;
        Dmg = ammunition.Dmg;
        APPower = ammunition.APPower;
        Caliber = ammunition.Caliber;

        crossSectionArea = Mathf.PI * Mathf.Pow(Caliber / 2000f, 2f);
        initialVelocity = fireDirection.normalized * ammunition.MuzzleVelocity;
    }

    void Start()
    {
        velocity = initialVelocity;
        k = 0.5f * airDensity * dragCoefficient * crossSectionArea;

        // Ha szükséges, induló "magasság" állítása (méterben)
        simulatedHeight = 1.2f;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Sebesség nagysága (csak X-Y síkban, felülnézetes játékhoz)
        Vector2 planarVelocity = new Vector2(velocity.x, velocity.y);
        float speed = planarVelocity.magnitude;

        // Légellenállás csak X-Y síkra
        Vector3 dragForce = -k * speed * new Vector3(velocity.x, velocity.y, 0f);

        // Gravitáció a szimulált magasságra (Z-tengely mentén)
        float gravityForceZ = -mass * gravity;
        float accelerationZ = gravityForceZ / mass;

        // Magasság frissítése (Z = szimulált magasság)
        simulatedHeight += velocity.z * dt;
        velocity.z += accelerationZ * dt;

        // Ha "leesett" a földre (szimuláltan)
        if (simulatedHeight < 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Vízszintes gyorsulás és sebesség
        Vector3 netForce = dragForce;
        Vector3 acceleration = netForce / mass;
        velocity.x += acceleration.x * dt;
        velocity.y += acceleration.y * dt;

        // Csak X-Y síkban mozog a GameObject, Z nem változik fizikailag
        transform.position += new Vector3(velocity.x, velocity.y, 0f) * dt;
    }
}