using CALIMOE;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orbarella;

public class NightmareSystem
{
    private List<Building> _buildings;
    private float _maxCityNightmareLevel;
    private float _cityNightmareLevel;
    private float _newDreamerTimer;
    private float _spawnRate;

    static Random _random = new Random();

    public float CityLevel => _cityNightmareLevel / _maxCityNightmareLevel;

    public NightmareSystem()
    {
    }

    public void LoadLevel(List<Building> buildings, float spawnRate, float nightmareFactor)
    {
        _spawnRate = spawnRate;
        _newDreamerTimer = spawnRate;
        _buildings = buildings;
        int numResidents = buildings.Sum(b => b.NumResidents);
        _maxCityNightmareLevel = numResidents * nightmareFactor;
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
                _newDreamerTimer = _spawnRate;
                return;
            }
            numBuildingsTested++;
        }
    }

}
