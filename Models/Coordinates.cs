// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodKept.Models
{
    public struct Coordinates
    {
        public double Lat { get; set; }
        public double Lng { get; set; }

        public Coordinates(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }

        public static Coordinates GetOrigin()
        {
            return new Coordinates();
        }
    }
}
