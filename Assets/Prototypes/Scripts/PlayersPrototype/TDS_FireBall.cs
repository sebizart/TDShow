using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_FireBall : MonoBehaviour
{
    #region Fields / Properties
    [SerializeField] private LayerMask whatCollides = new LayerMask();

    [SerializeField] private new Rigidbody rigidbody = null;
    #endregion

    #region Methods
    #region Original Methods
    public void Init(Vector3 _velocity)
    {
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();

        rigidbody.velocity = _velocity;
    }
    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        if (whatCollides == (whatCollides | (1 << other.gameObject.layer)))
        {
            if (other.GetComponent<TDS_DamageableElement>())
            {
                other.GetComponent<TDS_DamageableElement>().TakeDamage(Random.Range(7, 9));
            }

            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    #endregion
    #endregion
}
