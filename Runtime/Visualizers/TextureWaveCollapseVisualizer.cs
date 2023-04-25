using System.Linq;
using UnityEngine;

namespace Gameframe.Procgen
{
    public class TextureWaveCollapseVisualizer : MonoBehaviour
    {
        [SerializeField] private ColorWaveCollapseModelData modelData;

        [SerializeField] private Material _material;
        [SerializeField] private Texture2D _initTexture;

        [SerializeField] private int adjacentPixelMatchCount = 1;
        [SerializeField] private bool periodicInput = true;
        [SerializeField] private bool periodic = false;
        [SerializeField] [Range(1, 8)] private int symmetry = 8;
        [SerializeField] private bool ground = false;
        [SerializeField] private int groundTileIndex = 80;
        [SerializeField] private int groundY = 0;
        [SerializeField] private BaseWaveCollapseModel.Heuristic heuristic;

        [SerializeField] private int seed = 0;
        [SerializeField] private int limit = -1;

        [SerializeField] private Vector2Int outputSize = new Vector2Int(64, 64);

        [SerializeField]
        private TextureWaveCollapseModel _model;

        [SerializeField]
        private TextureWaveCollapseModel.TextureWaveEntry[] _wave;

        [SerializeField] private TextureWaveCollapseModel.TextureWaveCollapseTextureData propagatorData;

        [SerializeField] private KeyCode initAndRunKey = KeyCode.Return;
        [SerializeField] private KeyCode runKey = KeyCode.Space;
        [SerializeField] private KeyCode reseed = KeyCode.R;

        [SerializeField] private Texture2D[] patterns;

        private Texture2D _outputTexture;

        private void Start()
        {
            _model = new TextureWaveCollapseModel(_initTexture, adjacentPixelMatchCount, periodicInput, symmetry);
            _model.Init(seed, outputSize.x, outputSize.y, periodic, heuristic);
        }

        [ContextMenu("Init And Run")]
        public void InitAndRun()
        {
            _outputTexture = new Texture2D(outputSize.x, outputSize.y, TextureFormat.RGB24, false)
            {
                filterMode = FilterMode.Point
            };

            _model = new TextureWaveCollapseModel(_initTexture, adjacentPixelMatchCount, periodicInput, symmetry);
            _model.Init(seed, outputSize.x, outputSize.y, periodic, heuristic);
            patterns = _model.GetPatternsAsTextures().ToArray();

            modelData = _model.ColorModelData;

            if (ground)
            {
                _model.Ground(groundTileIndex, groundY);
            }

            if (!_model.Run(limit))
            {
                Debug.LogError("Contradiction!");
                return;
            }

            _model.Save(_outputTexture);
            _material.mainTexture = _outputTexture;
        }

        public void InitOnly()
        {
            _outputTexture = new Texture2D(outputSize.x, outputSize.y, TextureFormat.RGB24, false)
            {
                filterMode = FilterMode.Point
            };

            _model = new TextureWaveCollapseModel(_initTexture, adjacentPixelMatchCount, periodicInput, symmetry);
            _model.Init(seed, outputSize.x, outputSize.y, periodic, heuristic);
            patterns = _model.GetPatternsAsTextures().ToArray();

            modelData = _model.ColorModelData;

            if (ground)
            {
                Debug.Log($"Grounding: (0,{groundY}) = {groundTileIndex}");
                _model.Ground(groundTileIndex, groundY);
            }

            _model.Save(_outputTexture);
            _material.mainTexture = _outputTexture;
        }

        [ContextMenu("GetWave")]
        public void GetWave()
        {
            _wave = _model.GetWave();
        }

        [ContextMenu("GetPropagator")]
        public void GetPropagator()
        {
            propagatorData = _model.GetPropagatorData();
        }

        [ContextMenu("StepNext")]
        public void StepNext()
        {
            var result = _model.StepRun();
            if (result != null)
            {
                Debug.Log($"Done. Result = {result.Value}");
                return;
            }

            _model.Save(_outputTexture);
            _material.mainTexture = _outputTexture;
        }

        [ContextMenu("DrawCurrentOutput")]
        public void DrawCurrent()
        {
            _model.Save(_outputTexture);
            _material.mainTexture = _outputTexture;
        }

        [ContextMenu("DrawStepEntry")]
        public void DrawStepEntry()
        {
            //Draw the most recent collapsed & selected pattern to the texture
            var stepEntry = _model.collapseEntries[^1];
            var x = stepEntry.node % _model.OutputWidth;
            var y = stepEntry.node / _model.OutputWidth;
            var xMax = x + _model.ModelData.patternSize;
            var yMax = y + _model.ModelData.patternSize;

            if (xMax > _model.OutputWidth)
            {
                xMax = _model.OutputWidth;
            }

            if (yMax > _model.OutputHeight)
            {
                yMax = _model.OutputHeight;
            }

            var pattern = _model.PatternToPixels(stepEntry.patternIndex);

            for (int iy = y; iy < yMax; iy++)
            {
                for (int ix = x; ix < xMax; ix++)
                {
                    var px = ix - x;
                    var py = iy - y;
                    var pi = px + py * _model.ModelData.patternSize;
                    _outputTexture.SetPixel(ix,iy, pattern[pi]);
                }
            }

            _outputTexture.Apply();
            _material.mainTexture = _outputTexture;
        }

        [ContextMenu("DrawAllStepEntries")]
        public void DrawAllStepEntry()
        {
            //Draw the most recent collapsed & selected pattern to the texture
            for (int i = 0; i < _model.collapseEntries.Count; i++)
            {
                var stepEntry = _model.collapseEntries[i];
                var x = stepEntry.node % _model.OutputWidth;
                var y = stepEntry.node / _model.OutputWidth;
                var xMax = x + _model.ModelData.patternSize;
                var yMax = y + _model.ModelData.patternSize;

                if (xMax > _model.OutputWidth)
                {
                    xMax = _model.OutputWidth;
                }

                if (yMax > _model.OutputHeight)
                {
                    yMax = _model.OutputHeight;
                }

                var pattern = _model.PatternToPixels(stepEntry.patternIndex);

                for (int iy = y; iy < yMax; iy++)
                {
                    for (int ix = x; ix < xMax; ix++)
                    {
                        var px = ix - x;
                        var py = iy - y;
                        var pi = px + py * _model.ModelData.patternSize;
                        _outputTexture.SetPixel(ix,iy, pattern[pi]);
                    }
                }
            }
            _outputTexture.Apply();
            _material.mainTexture = _outputTexture;
        }

        [ContextMenu("Init-Run Reseed")]
        public void InitAndRunReseed()
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            InitAndRun();
        }

        // [ContextMenu("TestFlip")]
        // private void TestFlip()
        // {
        //     _outputTexture = new Texture2D(_initTexture.width, _initTexture.height, TextureFormat.RGB24, false)
        //     {
        //         filterMode = FilterMode.Point
        //     };
        //
        //     var pixels = _initTexture.GetPixels();
        //     pixels.FlipVertical(_initTexture.width, _initTexture.height);
        //     _outputTexture.SetPixels(pixels);
        //     _outputTexture.Apply();
        //
        //     _material.mainTexture = _outputTexture;
        // }

        private void Update()
        {


        }
    }
}
