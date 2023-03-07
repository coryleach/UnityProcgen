using System.Collections.Generic;
using System.Linq;
using Gameframe.Procgen;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class TextureWaveCollapseVisualizer : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private Texture2D _initTexture;

    [SerializeField] private int N = 3;
    [SerializeField] private bool periodicInput = true;
    [SerializeField] private bool periodic = false;
    [SerializeField][Range(1,8)] private int symmetry = 8;
    [SerializeField] private bool ground = false;
    [SerializeField] private WaveCollapseModel.Heuristic heuristic;

    [SerializeField] private int seed = 0;
    [SerializeField] private int limit = -1;

    [SerializeField] private Vector2Int outputSize = new Vector2Int(64,64);

    private TextureWaveCollapseModel _model;

    [SerializeField] private KeyCode initAndRunKey = KeyCode.Return;
    [SerializeField] private KeyCode runKey = KeyCode.Space;
    [SerializeField] private KeyCode reseed = KeyCode.R;

    [SerializeField] private List<Texture2D> patterns = new List<Texture2D>();

    private Texture2D _outputTexture;

    private void Start()
    {
        Init();
    }

    [ContextMenu("Init")]
    private void Init()
    {
        _model = new TextureWaveCollapseModel(_initTexture, N,outputSize.x, outputSize.y,periodicInput, periodic, symmetry, ground, heuristic);
        _outputTexture = new Texture2D(outputSize.x, outputSize.y, TextureFormat.RGB24, false)
        {
            filterMode = FilterMode.Point
        };
        _material.mainTexture = _outputTexture;
        patterns = _model.GetPatternsAsTextures().ToList();
        _model.InitRun(seed);
    }

    [ContextMenu("Run")]
    private void Run()
    {
        if ( !_model.Run(seed, limit) )
        {
            Debug.LogError("Contradiction!");
            return;
        }
        Debug.Log("Success");
        _model.Save(_outputTexture);
        _material.mainTexture = _outputTexture;
    }

    [ContextMenu("Init And Run")]
    private void InitAndRun()
    {
        Init();
        Run();
    }

    private void Update()
    {
        if (!_model.IsDone)
        {
            if (!_model.StepRun())
            {
                Debug.Log("Failed");
                _model.Save(_outputTexture);
                _material.mainTexture = _outputTexture;
            }
            else
            {
                _model.Save(_outputTexture);
                _material.mainTexture = _outputTexture;
            }
        }

        if (Input.GetKeyDown(reseed))
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            _model.InitRun(seed);
        }

        // if (Input.GetKeyDown(initAndRunKey))
        // {
        //     InitAndRun();
        // }
        // else if (Input.GetKeyDown(runKey))
        // {
        //     Run();
        // }
        // else if (Input.GetKeyDown(reseed))
        // {
        //     seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        //     InitAndRun();
        // }
    }

}
