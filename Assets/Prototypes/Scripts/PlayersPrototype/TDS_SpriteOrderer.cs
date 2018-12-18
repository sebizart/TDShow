using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDS_SpriteOrderer : MonoBehaviour
{
    #region Fields / Properties
    [Header("Sprites :")]
    // The list of sprite renderers to order
    [SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    [Header("Shadow :")]
    // The layer mask of the shadows
    [SerializeField] private LayerMask whatIsShadow = new LayerMask();
    #endregion

    #region Singleton
    // The singleton instance of this script
    [SerializeField] public static TDS_SpriteOrderer Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Adds a sprite to the list of Sprite Renderers being ordered
    /// </summary>
    /// <param name="_sprite">Sprite Renderer to add</param>
    public void AddSprite(SpriteRenderer _sprite)
    {
        if ((whatIsShadow != (whatIsShadow | (1 << _sprite.gameObject.layer))) && !sprites.Contains(_sprite))
        {
            sprites.Add(_sprite);
            _sprite.transform.forward = Camera.main.transform.forward;
        }
    }
    /// <summary>
    /// Adds an array of sprites to the list of Sprite Renderers being ordered
    /// </summary>
    /// <param name="_sprites">Array of Sprite Renderers to add</param>
    public void AddSprite(SpriteRenderer[] _sprites)
    {
        _sprites = _sprites.ToList().Where(s => (whatIsShadow != (whatIsShadow | (1 << s.gameObject.layer))) && !sprites.Contains(s)).ToArray();

        _sprites.ToList().ForEach(s => sprites.Add(s));
        _sprites.ToList().ForEach(s => s.transform.forward = Camera.main.transform.forward);
    }

    public void Order()
    {
        List<SpriteRenderer> _spritesToRemove = new List<SpriteRenderer>();

        foreach (SpriteRenderer _sprite in sprites)
        {
            if (_sprite == null)
            {
                _spritesToRemove.Add(_sprite);
            }
            else
            {
                _sprite.sortingOrder = -(int)Camera.main.WorldToScreenPoint(_sprite.transform.position).z;
            }
        }

        _spritesToRemove.ForEach(s => sprites.Remove(s));
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        Order();
    }

    // Use this for initialization
    void Start ()
    {
        // Get all scene sprites not in the list and add them to it
        SpriteRenderer[] _sprites = FindObjectsOfType<SpriteRenderer>().Where(s => (whatIsShadow != (whatIsShadow | (1 << s.gameObject.layer))) && !sprites.Contains(s)).ToArray();
        _sprites.ToList().ForEach(s => sprites.Add(s));

        _sprites.ToList().ForEach(s => s.transform.forward = Camera.main.transform.forward);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    #endregion
    #endregion
}
