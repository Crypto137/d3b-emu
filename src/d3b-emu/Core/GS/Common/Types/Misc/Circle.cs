﻿/*
 * Copyright (C) 2023 d3b-emu
 *
 * This program is free software: you can redistribute it and/or modify 
 * it under the terms of the GNU Affero General Public License as published by 
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program; if not, see <https://www.gnu.org/licenses/>
 */

using System.Drawing;
using D3BEmu.Core.GS.Common.Types.Math;

// based on: http://www.krissteele.net/blogdetails.aspx?id=251

namespace D3BEmu.Core.GS.Common.Types.Misc
{
    public struct Circle
    {
        /// <summary> 
        /// Center position of the circle. 
        /// </summary> 
        public Vector2F Center;

        /// <summary> 
        /// Radius of the circle. 
        /// </summary> 
        public float Radius;

        /// <summary> 
        /// Constructs a new circle. 
        /// </summary> 
        public Circle(Vector2F position, float radius)
        {
            this.Center = position;
            this.Radius = radius;
        }

        /// <summary> 
        /// Constructs a new circle. 
        /// </summary> 
        public Circle(float x, float y, float radius)
            :this(new Vector2F(x,y),radius)
        { }

        /// <summary> 
        /// Determines if a circle intersects a rectangle. 
        /// </summary> 
        /// <returns>True if the circle and rectangle overlap. False otherwise.</returns> 
        public bool Intersects(Rectangle rectangle)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = Clamp(this.Center.X, (float)rectangle.Left, (float)rectangle.Right);
            float closestY = Clamp(this.Center.Y, (float)rectangle.Top, (float)rectangle.Bottom);

            // Calculate the distance between the circle's center and this closest point
            float distanceX = this.Center.X - closestX;
            float distanceY = this.Center.Y - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (this.Radius * this.Radius);
        }

        public bool Intersects(RectangleF rectangle)
        {
            // Find the closest point to the circle within the rectangle
            float closestX = Clamp(Center.X, (float)rectangle.Left, (float)rectangle.Right);
            float closestY = Clamp(Center.Y, (float)rectangle.Top, (float)rectangle.Bottom);

            // Calculate the distance between the circle's center and this closest point
            float distanceX = Center.X - closestX;
            float distanceY = Center.Y - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (Radius * Radius);
        }

        public static float Clamp(float value, float min, float max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }
    }
}
