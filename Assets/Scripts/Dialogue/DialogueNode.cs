﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueID = "";
        public string text = "";
        public List<string> children = new List<string>();
        public Rect rect = new Rect(30, 30, 400, 150);
    }
}
