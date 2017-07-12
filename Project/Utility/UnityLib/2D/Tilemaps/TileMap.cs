using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBX.TileMapping.Unity
{
    using Lerp2API.Utility.UnityLib;
    using UnityEngine;
    using UnityEngine.WSA;

    /// <summary>
    /// Provides a component for tile mapping.
    /// </summary>
    public class TileMap : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the number of rows of tiles.
        /// </summary>
        public int Rows;

        /// <summary>
        /// Gets or sets the number of columns of tiles.
        /// </summary>
        public int Columns;

        /// <summary>
        /// Gets or sets the value of the tile width.
        /// </summary>
        public float TileWidth = 1;

        /// <summary>
        /// Sets the tile.
        /// </summary>
        /// <param name="tilePos">The tile position.</param>
        /// <param name="tile">The tile.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetTile(Vector3Int tilePos, Tile tile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the value of the tile height.
        /// </summary>
        public float TileHeight = 1;

        /// <summary>
        /// Used by editor components or game logic to indicate a tile location.
        /// </summary>
        /// <remarks>This will be hidden from the inspector window. See <see cref="HideInInspector"/></remarks>
        [HideInInspector]
        public Vector3 MarkerPosition;

        /// <summary>
        /// The color
        /// </summary>
        public Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMap"/> class.
        /// </summary>
        public TileMap()
        {
            this.Columns = 20;
            this.Rows = 10;
        }

        /// <summary>
        /// When the game object is selected this will draw the grid
        /// </summary>
        /// <remarks>Only called when in the Unity editor.</remarks>
        private void OnDrawGizmosSelected()
        {
            // store map width, height and position
            var mapWidth = this.Columns * this.TileWidth;
            var mapHeight = this.Rows * this.TileHeight;
            var position = this.transform.position;

            // draw layer border
            Gizmos.color = Color.white;
            Gizmos.DrawLine(position, position + new Vector3(mapWidth, 0, 0));
            Gizmos.DrawLine(position, position + new Vector3(0, mapHeight, 0));
            Gizmos.DrawLine(position + new Vector3(mapWidth, 0, 0), position + new Vector3(mapWidth, mapHeight, 0));
            Gizmos.DrawLine(position + new Vector3(0, mapHeight, 0), position + new Vector3(mapWidth, mapHeight, 0));

            // draw tile cells
            Gizmos.color = Color.grey;
            for (float i = 1; i < this.Columns; i++)
            {
                Gizmos.DrawLine(position + new Vector3(i * this.TileWidth, 0, 0), position + new Vector3(i * this.TileWidth, mapHeight, 0));
            }

            for (float i = 1; i < this.Rows; i++)
            {
                Gizmos.DrawLine(position + new Vector3(0, i * this.TileHeight, 0), position + new Vector3(mapWidth, i * this.TileHeight, 0));
            }

            // Draw marker position
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.MarkerPosition, new Vector3(this.TileWidth, this.TileHeight, 1) * 1.1f);
        }
    }
}