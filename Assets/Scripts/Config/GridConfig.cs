using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GridConfig", menuName = "Config/Grid")]
public class GridConfig : ScriptableObject
{
    public int cols;
    public int startFilledCell;
    public int duplicateNumber;
    public int maxValueNumber; // Number of cell
}
