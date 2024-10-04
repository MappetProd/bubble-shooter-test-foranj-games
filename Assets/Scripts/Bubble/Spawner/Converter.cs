using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class Converter : MonoBehaviour
{
    const string ENPTY_LINE_REGEX = @"^(?=\s*$)";

    StreamReader reader;

    List<char[]> level;
    Dictionary<char, Bubble> charBubbleCodes;
    Dictionary<Bubble, float> bubbleSpawnProbabilities;
    Vector2 startPos;
    

    enum Bubble { RED, GREEN, BLUE, RANDOM, VOID }

    private void Awake()
    {
        string LEVEL_FILE = $"{Application.dataPath}/Levels/1_level.txt";

        charBubbleCodes = new Dictionary<char, Bubble>();
        bubbleSpawnProbabilities = new Dictionary<Bubble, float>();
        level = new List<char[]>();
        startPos = Vector2.zero;

        reader = new StreamReader(LEVEL_FILE);

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
        foreach (KeyValuePair<Bubble, float> pair in bubbleSpawnProbabilities)
        {
            totalChance += pair.Value;
        }

        return totalChance <= 1;  
    }

    private void InterpretBubbleCodeLine(string line)
    {
        string[] bubbleTypes = Enum.GetNames(typeof(Bubble));
        string[] expression = line.Split(' ');
        if (bubbleTypes.Contains(expression[2]))
        {
            Bubble bubbleEnumByString = (Bubble)Enum.Parse(typeof(Bubble), expression[2]);
            charBubbleCodes.Add(char.Parse(expression[0]), bubbleEnumByString);
        }
        else
        {
            throw new Exception($"Syntax error. There is no such bubble type as @{expression[2]} in bubblecodes block");
        }
    }

    private void InterpretBubbleSpawnProbability(string line)
    {
        string[] bubbleTypes = Enum.GetNames(typeof(Bubble));

        string[] expression = line.Split(' ');
        string bubbleType = expression[0];
        string probability = expression[2];

        if (bubbleTypes.Contains(bubbleType))
        {
            Bubble bubbleEnumByString = (Bubble)Enum.Parse(typeof(Bubble), bubbleType);
            bubbleSpawnProbabilities.Add(bubbleEnumByString, Single.Parse(probability, CultureInfo.InvariantCulture));
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

        level.Add(levelRow);
    }

    private bool CheckRowForNonDeclaredBubbleTypes(char[] levelRow)
    {
        foreach (char c in levelRow)
        {
            if (!charBubbleCodes.ContainsKey(c))
                return false;
        }
        return true;
    }
    private void InterpretStartPositionLine(string line)
    {
        string[] expression = line.Split(' ');

        if (expression[0] == "x")
            startPos.x = Single.Parse(expression[2], CultureInfo.InvariantCulture);
        else if (expression[0] == "y")
            startPos.y = Single.Parse(expression[2], CultureInfo.InvariantCulture);
        else
            throw new System.Exception($"Syntax error in startposition block");
    }

    /*private Dictionary<Bubble, float> ParseProbabilities()
    {
        Dictionary<Bubble, float> result = new Dictionary<Bubble, float>();
        string line = reader.ReadLine();
        string[] bubbleTypes = Enum.GetNames(typeof(Bubble));

        while (line != "endrandom" && !reader.EndOfStream)
        {
            string[] expression = line.Split(' ');
            string bubbleType = expression[0];
            string probability = expression[2];

            if (bubbleTypes.Contains(bubbleType))
            {
                Bubble bubbleEnumByString = (Bubble)Enum.Parse(typeof(Bubble), bubbleType);
                result.Add(bubbleEnumByString, float.Parse(probability));
            }
        }

        return result;
    }*/


    /*private Dictionary<char, Bubble> ParseBubbleCodes()
    {
        Dictionary<char, Bubble> result = new Dictionary<char, Bubble>();
        string line = reader.ReadLine();
        string[] bubbleTypes = Enum.GetNames(typeof(Bubble));

        while (line != "endbubblecodes" && !reader.EndOfStream)
        {
            string[] expression = line.Split(' ');
            if (bubbleTypes.Contains(expression[2]))
            {
                Bubble bubbleEnumByString = (Bubble)Enum.Parse(typeof(Bubble), expression[2]);
                result.Add(char.Parse(expression[0]), bubbleEnumByString);
            }
            else
            {
                throw new Exception($"Syntax error. There is no such bubble type as @{expression[2]}");
            }

            line = reader.ReadLine();
        }
        return result;
    }*/

    /*private List<char[]> ParseLevel()
    {
        string line = reader.ReadLine();
        List<char[]> level = new List<char[]>();
        while (line != "endlevel" && !reader.EndOfStream)
        {
            if (!Regex.IsMatch(line, ENPTY_LINE_REGEX))
            {
                char[] levelRow = line.ToCharArray();

                bool isRowHasInvalidBubbleType = CheckRowForNonDeclaredBubbleTypes(levelRow);
                if (isRowHasInvalidBubbleType)
                    throw new Exception("There is a non declared bubble type char in 'level' block");

                level.Add(levelRow);
            }
            
            line = reader.ReadLine();
        }
        return level;
    }*/

    /*private Vector2 ParseStartPosition()
    {
        Vector2 startPos = new Vector2();


        string line = reader.ReadLine();
        while (line != "endstartposition" && !reader.EndOfStream)
        {
            if (!Regex.IsMatch(line, ENPTY_LINE_REGEX))
                InterpretStartPositionLine(line, ref startPos);
            line = reader.ReadLine();
        }

        return startPos;
    }*/


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
