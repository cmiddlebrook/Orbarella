using CALIMOE;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orbarella;

public class NightmareSystem
{
    // these values will later be refactored to be loaded in from a level definition
    private const float NEW_NIGHTMARE_SPAWN = 2.2f;
    private const float MAX_NIGHTMARE_FACTOR = 0.8f;

    private List<Building> _buildings;
    private float _maxCityNightmareLevel;
    private float _cityNightmareLevel;
    private float _newDreamerTimer = NEW_NIGHTMARE_SPAWN;

    static Random _random = new Random();

    public float CityLevel => _cityNightmareLevel / _maxCityNightmareLevel;

    public NightmareSystem(List<Building> buildings)
    {
        _buildings = buildings;
        int numResidents = buildings.Sum(b => b.NumResidents);
        _maxCityNightmareLevel = numResidents * MAX_NIGHTMARE_FACTOR;
    }


    public void Update(GameTime gt)
    {
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        _newDreamerTimer -= delta;
        if (_newDreamerTimer < 0)
        {
            StartNightmare();
        }

        _cityNightmareLevel = 0f;
        foreach (Building building in _buildings)
        {
            building.Update(gt);
            _cityNightmareLevel += building.NumDreaming;
        }
    }

    public void StartNightmare()
    {
        int numBuildingsTested = 0;
        int maxBuildings = _buildings.Count - 1; // -1 to account for the blank house on every level
        bool newNightmare = false;
        while (!newNightmare && numBuildingsTested < maxBuildings)
        {
            var buildingIdx = _random.Next(maxBuildings);
            newNightmare = _buildings[buildingIdx].StartNightmare();
            if (newNightmare)
            {
                _newDreamerTimer = _newDreamerTimer = NEW_NIGHTMARE_SPAWN;
                return;
            }
            numBuildingsTested++;
        }
    }

}
