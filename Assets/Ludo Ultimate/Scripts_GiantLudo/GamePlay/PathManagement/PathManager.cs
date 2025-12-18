using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoTemplate
{

    #region  PublicVariables
    public static OnPathCompleteSetup onPathCompleteSetup;
    #endregion

    #region PrivateRegions
    [SerializeField] private PathElement[] path;
    [SerializeField] private PathElement[] topRightHome;
    [SerializeField] private PathElement[] bottomRightHome;
    [SerializeField] private PathElement[] bottomLeftHome;
    [SerializeField] private PathElement[] topLeftHome;

    [Space]
    [SerializeField] private Color lineColor;
    [SerializeField] private Color elementsColor;

    [Range(0, 1)]
    [SerializeField] private float radius;
    [SerializeField] private PathType showPath;
    private int topRightPathStartIndex = 0;
    private int bottomRightPathStartIndex = 0;
    private int bottomLeftPathStartIndex = 0;
    private int topLeftPathStartIndex = 0;
    #endregion

    #region Unity_Callback

    private void Awake()
    {
    }
    void OnEnable()
    {
        SetPathStartPoints();
        SetPathForPlayers();
    }
    private void OnDrawGizmos()
    {
        SetPathStartPoints();
        PathElement[] pes = GetPathForPawns(showPath);

        DrawPathElements(pes);
        /* DrawPathElements(topRightHome);
        DrawPathElements(bottomRightHome);
        DrawPathElements(bottomLeftHome);
        DrawPathElements(topLeftHome); */
    }
    #endregion

    #region PublicMethods
    public PathElement[] GetPathForPawns(PathType pathType)
    {
        PathElement[] elements = null;
        switch (pathType)
        {
            case PathType.topRight:
                elements = GetPathFromstartPoint(topRightPathStartIndex, pathType);
                break;

            case PathType.bottomRight:
                elements = GetPathFromstartPoint(bottomRightPathStartIndex, pathType);
                break;

            case PathType.bottomLeft:
                elements = GetPathFromstartPoint(bottomLeftPathStartIndex, pathType);
                break;

            case PathType.topLeft:
                elements = GetPathFromstartPoint(topLeftPathStartIndex, pathType);
                break;
        }
        return elements;
    }


    public PathElement[] GetPathForPawns_miniboard(PathType pathType)
    {
        PathElement[] elements = null;
        switch (pathType)
        {
            case PathType.topRight:
                elements = GetPathFromstartPoint_MiniBoard(topRightPathStartIndex, pathType);
                break;

            case PathType.bottomRight:
                elements = GetPathFromstartPoint_MiniBoard(bottomRightPathStartIndex, pathType);
                break;

            case PathType.bottomLeft:
                elements = GetPathFromstartPoint_MiniBoard(bottomLeftPathStartIndex, pathType);
                break;

            case PathType.topLeft:
                elements = GetPathFromstartPoint_MiniBoard(topLeftPathStartIndex, pathType);
                break;
        }
        return elements;
    }
    #endregion

    #region Private_Methods
    private void SetPathForPlayers()
    {
        PlayerData[] data = GetGameScreen.GetPlayers;
        foreach (PlayerData item in data)
        {
            item.path = GetPathForPawns(item.PathType);
            item.CLearPath();
        }

        onPathCompleteSetup?.Invoke();
    }

    private PathElement[] GetPathFromstartPoint(int index, PathType type)
    {
        List<PathElement> elements = new List<PathElement>();
        int offset = -1;
        for (int i = 0, diversanIndex = 0; i < path.Length; i++)
        {
            int ni = i + index;
            if (ni >= path.Length)
            {
                if (offset == -1) offset = i;
                ni = i - offset;
            }

            PathElement pe = path[ni];
            elements.Add(pe);
            if (pe.type == ElementType.diversen)
            {
                diversanIndex++;
            }
            if (diversanIndex == 4)
            {
                break;
            }
        }

        elements.AddRange(GetHomePath(type));
        return elements.ToArray();
    }

    private PathElement[] GetPathFromstartPoint_MiniBoard(int index, PathType type)
    {
        List<PathElement> elements = new List<PathElement>();
        int offset = -1;
        for (int i = 0, diversanIndex = 0; i < path.Length; i++)
        {
            int ni = i + index;
            if (ni >= path.Length)
            {
                if (offset == -1) offset = i;
                ni = i - offset;
            }

            PathElement pe = path[ni];
            elements.Add(pe);
            if (pe.type == ElementType.diversen)
            {
                diversanIndex++;
            }
            if (diversanIndex == 4)
            {
                break;
            }
        }

        elements.AddRange(GetHomePath(type));
        return elements.ToArray();
    }


    private PathElement[] GetHomePath(PathType type)
    {
        PathElement[] elements = null;
        switch (type)
        {
            case PathType.topRight:
                elements = topRightHome;
                break;

            case PathType.bottomRight:
                elements = bottomRightHome;
                break;

            case PathType.bottomLeft:
                elements = bottomLeftHome;
                break;

            case PathType.topLeft:
                elements = topLeftHome;
                break;
        }

        return elements;
    }
    private void DrawPathElements(PathElement[] path)
    {
        if (path.Length <= 0)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(path[0].transform.position, radius);

        for (int i = 1; i < path.Length; i++)
        {

            Gizmos.color = lineColor;
            Gizmos.DrawLine(path[i - 1].transform.position, path[i].transform.position);

            Gizmos.color = (path[i].type == ElementType.immortalLand) ? Color.cyan : elementsColor;
            Gizmos.DrawSphere(path[i].transform.position, radius);
        }
    }

    private void SetPathStartPoints()
    {
        for (int i = 0; i < path.Length; i++)
        {
            PathElement pe = path[i].GetComponent<PathElement>();
            switch (pe.startPointOf)
            {
                case PathType.topRight:
                    topRightPathStartIndex = i;
                    break;

                case PathType.topLeft:
                    topLeftPathStartIndex = i;
                    break;

                case PathType.bottomRight:
                    bottomRightPathStartIndex = i;
                    break;

                case PathType.bottomLeft:
                    bottomLeftPathStartIndex = i;
                    break;
            }
        }
    }
    #endregion
}
public delegate void OnPathCompleteSetup();