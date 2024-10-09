using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class Converter : MonoBehaviour
{
    const string ENPTY_LINE_REGEX = @"^(?=\s*$)";

    private static Converter _instance;

    public static Converter Instance
    {
        get
        {
            if ( _instance == null)
            {
                _instance = new GameObject("TXTConverter").AddComponent<Converter>();
            }
            return _instance;
        }
    }

    public List<char[]> Level { get; private set; }
    public Dictionary<char, BubbleColor> BubbletypeByCharcode { get; private set; }
    public Dictionary<BubbleColor, float> ProbabilityByBubbletype { get; private set; }
    public Vector2 StartSpawnPosition { get; private set; }

    private StreamReader reader;
    //public static event Action LevelConvertedFromTxt;
    public static event Func<IEnumerator> LevelConvertedFromTxt;

    public enum BubbleColor { RED, GREEN, BLUE, RANDOM, VOID }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    private IEnumerator Start()
    {
        string LEVEL_FILE = $"{Application.dataPath}/Levels/1_level.txt";
        ConvertLevelFile(LEVEL_FILE);

        yield return new WaitUntil(() => LevelConvertedFromTxt != null);
        StartCoroutine(LevelConvertedFromTxt.Invoke());
    }

    private void ResetValues()
    {
        BubbletypeByCharcode = new Dictionary<char, BubbleColor>();
        ProbabilityByBubbletype = new Dictionary<BubbleColor, float>();
        Level = new List<char[]>();
        StartSpawnPosition = Vector2.zero;
    }

    private void ConvertLevelFile(string filepath)
    {
        ResetValues();
        reader = new StreamReader(filepath);

        while (!reader.EndOfStream)
        {
            string rawBlockName = reader.ReadLine();
            string blockName = Regex.Replace(rawBlockName, @"\s", string.Empty);

            switch (blockName)
            {
                case "level":
                    ParseBlock(blockName, InterpretLevelLine);
                    break;
                case "startposition":
                    ParseBlock(blockName, InterpretStartPositionLine);
                    break;
                case "bubblecodes":
                    ParseBlock(blockName, InterpretBubbleCodeLine);
                    break;
                case "random":
                    ParseBlock(blockName, InterpretBubbleSpawnProbability);
                    bool isValid = AreProbabilitiesValid();
                    if (!isValid)
                        throw new Exception("The sum of bubble spawn probability chances is more than 1");

                    break;
                default:
                    break;
            }
        }

        reader.Close();
    }


    private void ParseBlock(string blockName, Action<string> InterpretLine)
    {
        string line = reader.ReadLine();
        while (!Regex.IsMatch(line, @"end" + blockName) && !reader.EndOfStream)
        {
            if (!Regex.IsMatch(line, ENPTY_LINE_REGEX))
            {
                InterpretLine(line);
            }
            line = reader.ReadLine();
        }
    }

    private bool AreProbabilitiesValid()
    {
        float totalChance = 0;
        foreach (KeyValuePair<BubbleColor, float> pair in ProbabilityByBubbletype)
        {
            totalChance += pair.Value;
        }

        return totalChance <= 1;  
    }

    private void InterpretBubbleCodeLine(string line)
    {
        string[] bubbleTypes = Enum.GetNames(typeof(BubbleColor));
        string[] expression = line.Split(' ');
        if (bubbleTypes.Contains(expression[2]))
        {
            BubbleColor bubbleEnumByString = (BubbleColor)Enum.Parse(typeof(BubbleColor), expression[2]);
            char bubbleCode = char.Parse(expression[0]);
            if (BubbletypeByCharcode.ContainsKey(bubbleCode))
                throw new Exception($"There is a duplicate of {bubbleCode} code in bubblecodes block");

            BubbletypeByCharcode.Add(bubbleCode, bubbleEnumByString);
        }
        else
        {
            throw new Exception($"Syntax error. There is no such bubble type as @{expression[2]} in bubblecodes block");
        }
    }

    private void InterpretBubbleSpawnProbability(string line)
    {
        string[] bubbleTypes = Enum.GetNames(typeof(BubbleColor));

        string[] expression = line.Split(' ');
        string bubbleType = expression[0];
        string probability = expression[2];

        if (bubbleTypes.Contains(bubbleType))
        {
            BubbleColor bubbleEnumByString = (BubbleColor)Enum.Parse(typeof(BubbleColor), bubbleType);

            if (ProbabilityByBubbletype.ContainsKey(bubbleEnumByString))
                throw new Exception($"There is a duplicate of {nameof(bubbleEnumByString)} type in random block");

            ProbabilityByBubbletype.Add(bubbleEnumByString, Single.Parse(probability, CultureInfo.InvariantCulture));
        }
        else
            throw new Exception($"Syntax error. There is no such bubble type as @{bubbleType} in random block");
    }

    private void InterpretLevelLine(string line)
    {
        char[] levelRow = line.ToCharArray();

        bool isRowHasInvalidBubbleType = CheckRowForNonDeclaredBubbleTypes(levelRow);
        if (isRowHasInvalidBubbleType)
            throw new Exception("There is a non declared bubble type char in 'level' block");

        Level.Add(levelRow);
    }

    private bool CheckRowForNonDeclaredBubbleTypes(char[] levelRow)
    {
        foreach (char c in levelRow)
        {
            if (!BubbletypeByCharcode.ContainsKey(c))
                return false;
        }
        return true;
    }
    private void InterpretStartPositionLine(string line)
    {
        string[] expression = line.Split(' ');

        if (expression[0] == "x")
        {
            float newX = Single.Parse(expression[2], CultureInfo.InvariantCulture);
            StartSpawnPosition = new Vector2(newX, StartSpawnPosition.y);
        }
        else if (expression[0] == "y")
        {
            float newY = Single.Parse(expression[2], CultureInfo.InvariantCulture);
            StartSpawnPosition = new Vector2(StartSpawnPosition.x, newY);
        }
        else
            throw new System.Exception($"Syntax error in startposition block");
    }
}
