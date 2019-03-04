// Made with the audio visulizer tutorial by N3K EN.
using UnityEngine;

public class SourceAnalyze : MonoBehaviour
{
    #region Public members 

    [Header("Background Parameter")]
    public float m_backgroundIntensity;
    public Material m_backgroundMaterial;
    public Color m_minColor;
    public Color m_maxColor;

    [Header("Cube Parameter")]
    public GameObject m_cube;
    public float m_cubeIntensity;
    public Material m_cubeMaterial;
    public Color m_cubeMin;
    public Color m_cubeMax;

    public float m_rmsValue; // avg power input of the sound;
    public float m_dbValue;
    public float m_pitchValue;

    public float m_maxVisualScale = 25.0f;
    public float m_visualModifier = 50.0f;
    public float m_smoothSpeed = 10.0f;
    public float m_keepPercentage = 0.5f;

    #endregion


    #region System methods

    private void Start()
    {
        _tr = GetComponent<Transform>();
        _source = GetComponent<AudioSource>();
        _samples = new float[SAMPLE_SIZE];
        _spectrum = new float[SAMPLE_SIZE];
        _sampleRate = AudioSettings.outputSampleRate;
        //SpawnLine();
        SpawnCircle();
    }

    private void Update()
    {
        AnalyzeSound();
        UpdateVisual();
        UpdateBackground();
        UpdateCube();

        if(m_backgroundIntensity < m_dbValue/45)
        {
            _tr.RotateAround(Vector3.zero, new Vector3(0,0,1), 10 * Time.deltaTime);
        }
        else if (m_backgroundIntensity > m_dbValue/45)
        {
            _tr.RotateAround(Vector3.zero, new Vector3(0, 0, -1), 25 * Time.deltaTime);
        }
    }

    #endregion


    #region Main methods 

    private void AnalyzeSound()
    {
        _source.GetOutputData(_samples, 0);
        //GetTheRMS
        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++)
        {
            sum = _samples[i] * _samples[i];
        }
        m_rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        // Get the db value 
        m_dbValue = 20 * Mathf.Log10(m_rmsValue / 0.1f);

        // GetSoundSpectrum / Visualization
        _source.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
    }

    private void SpawnLine()
    {
        _visualScale = new float[_amountVisual];
        _visualList = new Transform[_amountVisual];

        for (int i = 0; i < _amountVisual; i++)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            _visualList[i] = obj.transform;
            _visualList[i].position = Vector3.right * i;
        }
    }
    private void SpawnCircle()
    {
        _visualScale = new float[_amountVisual];
        _visualList = new Transform[_amountVisual];

        Vector3 center = Vector3.zero;
        float radius = 20; // distance btw center and cubes

        for (int i = 0; i < _amountVisual; i++)
        {
            float ang = i * 1.0f / _amountVisual;
            ang = ang * Mathf.PI * 2;

            float x = center.x + Mathf.Cos(ang) * radius;
            float y = center.y + Mathf.Sin(ang) * radius;

            Vector3 pos = center + new Vector3(x, y, 0);
            GameObject obj = Instantiate(m_cube, pos, Quaternion.LookRotation(Vector3.forward, pos));
            obj.transform.parent = gameObject.transform;
            _visualList[i] = obj.transform;
        }
    }

    private void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int)(SAMPLE_SIZE * m_keepPercentage) / _amountVisual;

        while (visualIndex < _amountVisual)
        {
            int j = 0;
            float sum = 0;

            while (j < averageSize)
            {
                sum += _spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageSize * m_visualModifier;
            _visualScale[visualIndex] -= Time.deltaTime * 15;
            if (_visualScale[visualIndex] < scaleY)
            {
                _visualScale[visualIndex] = scaleY;
            }

            if (_visualScale[visualIndex] > m_maxVisualScale)
            {
                _visualScale[visualIndex] = m_maxVisualScale; // can't go further than this 
            }

            _visualList[visualIndex].localScale = Vector3.one + Vector3.up * _visualScale[visualIndex];
            visualIndex++;
        }
    }
    private void UpdateBackground()
    {
        m_backgroundIntensity -= Time.deltaTime * m_smoothSpeed;
        if (m_backgroundIntensity < m_dbValue / 40)
        {
            m_backgroundIntensity = m_dbValue / 40;
        }
        m_backgroundMaterial.color = Color.Lerp(m_maxColor, m_minColor, -m_backgroundIntensity);
    }
    private void UpdateCube()
    {
        m_cubeIntensity -= Time.deltaTime * m_smoothSpeed;
        if(m_cubeIntensity < m_dbValue /30)
        {
            m_cubeIntensity = m_dbValue / 40;
        }
        m_cubeMaterial.color = Color.Lerp(m_cubeMax, m_cubeMin, -m_cubeIntensity);
    }

    #endregion


    #region Private and protected members

    private const int SAMPLE_SIZE = 1024;
    private AudioSource _source;
    private float[] _samples;
    private float[] _spectrum;
    private float _sampleRate;

    // Visualization with cubes. 
    private Transform[] _visualList;
    private float[] _visualScale;
    private int _amountVisual = 100; // how many cubes. 

    private Transform _tr;

    #endregion
}
