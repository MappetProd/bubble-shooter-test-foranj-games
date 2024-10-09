using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ConvertedLevel
{
    public List<char[]> charLevelMap;
    public Dictionary<char, BubbleColor> bubbletypeByCharcode;
    public Dictionary<BubbleColor, float> probabilityByBubbletype;
    public Vector2 startSpawnPosition;
}
