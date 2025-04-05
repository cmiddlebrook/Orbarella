using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orbarella;

public class NightmareSystem
{
    private const float CITY_NIGHTMARE_RATE = 1.5f;
    private const float NEW_NIGHTMARE_SPAWN = 2.2f;

    private List<Building> _buildings;
    private float _cityNightmareLevel;
    private float _newDreamerTimer = NEW_NIGHTMARE_SPAWN;
    private NightmareMeter _nightmareMeter;
    private float _nightmareIncrement;

    static Random _random = new Random();

    public bool ThresholdReached => _nightmareMeter.IsFull;

    public NightmareSystem(AssetManager am, List<Building> buildings)
    {
        _buildings = buildings;
        _nightmareMeter = new NightmareMeter(am);
        int numResidents = buildings.Sum(b => b.NumResidents);
        _nightmareIncrement = _nightmareMeter.NumTicks / numResidents * CITY_NIGHTMARE_RATE;
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
            _cityNightmareLevel += (building.NumDreaming * _nightmareIncrement);
        }
        _nightmareMeter.SetLevel(_cityNightmareLevel);
        _nightmareMeter.Update(gt);

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

    public void Draw(SpriteBatch sb)
    {
        _nightmareMeter.Draw(sb);
    }
}
