﻿
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방 "내부" 정보
/// </summary>
public class RoomData
{
    public Vector2Int pos;
    public int interialNum; // 방 내부 장식 정보
    public List<(string characterName, Vector3 offsetPos)> enemies = new();
}