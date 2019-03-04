using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    #region Public members 

    public GameObject m_targetPlayer;
    public Transform m_pistol;
    public float m_orbitDistance;
    public float m_speed;
    public Vector3 m_orbit;

    #endregion


    #region System methods

    private void Awake()
    {
        _tr = m_pistol;
    }

    private void LateUpdate()
    {
        Orbit();
    }

    #endregion


    #region Main methods

    private void Orbit()
    {
        _tr.RotateAround(m_targetPlayer.transform.position, m_orbit, 100 * Time.deltaTime * m_speed);
    }

    #endregion 


    #region Private and protected members 

    private Transform _tr;

    #endregion
}
