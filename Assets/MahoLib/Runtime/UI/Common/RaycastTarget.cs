using UnityEngine.UI;
using UnityEngine;

namespace Maho.UI.Common
{
    /// <summary>
    /// Raycast検知
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class RaycastTarget : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
