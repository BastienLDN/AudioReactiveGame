﻿using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region Public members 

    public float m_speed;
    public Transform m_tr; // Enemy get the player position

    #endregion


    #region System methods

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        m_tr = GetComponent<Transform>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        _rb.velocity = new Vector2(horizontal, vertical) * m_speed * Time.deltaTime;
    }

    #endregion


    #region Private and protected members 

    private Rigidbody _rb;

    #endregion
}
