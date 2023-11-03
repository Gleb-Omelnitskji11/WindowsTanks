using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_Rigidbody;
    public bool IsBusy { get; private set; }

    public void Shoot(float bulletSpeed)
    {
        IsBusy = true;
        m_Rigidbody.velocity = transform.up * bulletSpeed;
    }

    public void TurnOff()
    {
        m_Rigidbody.velocity = Vector2.zero;
        gameObject.SetActive(false);
        IsBusy = false;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TurnOff();
    }
}