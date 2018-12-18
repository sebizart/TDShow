using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_MysteryBall : MonoBehaviour
{
    /* MysteryBall :
	*
	* [Write the Description of the Script here !]
	*/

    #region Events

    #endregion

    #region Fields / Accessors
    // The damages of the ball
    [SerializeField] private int damages = 1;

    // What the object collides on
    [SerializeField] private LayerMask whatCollides = new LayerMask();

    // The rigidbody of the object
    [SerializeField] private new Rigidbody rigidbody = null;
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Initializes the mystery ball by a velocity and an amount of damages to deal
    /// </summary>
    /// <param name="_velocity">Velocity to give to thsi object rigidbody</param>
    /// <param name="_damages">Damages to deal with the object</param>
    public void Init(Vector3 _velocity, int _damages)
    {
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = _velocity;
        damages = _damages;
    }
    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (whatCollides == (whatCollides | (1 << other.gameObject.layer)))
        {
            if (other.GetComponent<TDS_DamageableElement>())
            {
                other.GetComponent<TDS_DamageableElement>().TakeDamage(damages);
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
