﻿using Cardamom.Collections;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class PlayerKnowledge
    {
        public EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        public EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }

        private readonly Player _player;
        private readonly Map _map;
        private readonly AssetKnowledge _assetKnowledge;
        private readonly MapKnowledge _mapKnowledge;

        public PlayerKnowledge(Player player, Map map, AssetKnowledge assetKnowledge, MapKnowledge mapKnowledge)
        {
            _player = player;
            _map = map;
            _assetKnowledge = assetKnowledge;
            _mapKnowledge = mapKnowledge;
        }
        
        public SingleAssetKnowledge GetAsset(IAsset asset)
        {
            return _assetKnowledge.Get(asset);
        }

        public SingleTileKnowledge GetTile(Vector3i hex)
        {
            return _mapKnowledge.Get(hex);
        }

        public void Add(IAsset asset, Vector3i position, MultiMap<Vector3i, IAsset> positions)
        {
            if (asset is Unit unit && unit.Player == _player)
            {
                AddSelf(unit, position, positions);
            }
            else
            {
                AddOther(asset, position);
            }
        }

        public void Move(IAsset asset, Pathing.Path path, MultiMap<Vector3i, IAsset> positions)
        {
            if (asset is Unit unit && unit.Player == _player)
            {
                MoveSelf(unit, path, positions);
            }
            else
            {
                MoveOther(asset, path);
            }
        }

        public void Remove(IAsset asset, MultiMap<Vector3i, IAsset> positions)
        {
            if (asset is Unit unit && unit.Player == _player)
            {
                RemoveSelf(unit, positions);
            }
            else
            {
                RemoveOther(asset);
            }
        }

        private void AddOther(IAsset asset, Vector3i position)
        {
            var assetDelta = _assetKnowledge.AddOther(_mapKnowledge, asset, position);
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void AddSelf(Unit unit, Vector3i position, MultiMap<Vector3i, IAsset> positions)
        {
            var delta = Sighting.GetSightField(_map, position, GetMaxRange(unit)).ToList();
            var mapDelta = _mapKnowledge.Place(unit, delta);
            var assetDelta = _assetKnowledge.AddSelf(_mapKnowledge, unit, delta, positions);
            if (mapDelta.Any())
            {
                MapKnowledgeChanged?.Invoke(this, new(_player, mapDelta));
            }
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void MoveOther(IAsset asset, Pathing.Path path)
        {
            var assetDelta = _assetKnowledge.MoveOther(_mapKnowledge, asset, path);
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void MoveSelf(Unit unit, Pathing.Path path, MultiMap<Vector3i, IAsset> positions)
        {
            var maxRange = GetMaxRange(unit);
            var initial = Sighting.GetSightField(_map, path.Origin, maxRange).ToHashSet();
            var medial =
                path.Steps.Take(path.Steps.Count - 1)
                    .SelectMany(x => Sighting.GetSightField(_map, x, maxRange))
                    .ToHashSet();
            var final = Sighting.GetSightField(_map, path.Destination, maxRange).ToHashSet();

            var mapDelta = _mapKnowledge.Move(unit, initial, medial, final);
            var assetDelta = _assetKnowledge.MoveSelf(_mapKnowledge, unit, initial, medial, final, positions);
            if (mapDelta.Any())
            {
                MapKnowledgeChanged?.Invoke(this, new(_player, mapDelta));
            }
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void RemoveOther(IAsset asset)
        {
            var assetDelta = _assetKnowledge.RemoveOther(asset);
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void RemoveSelf(Unit unit, MultiMap<Vector3i, IAsset> positions)
        {
            var delta = Sighting.GetSightField(_map, unit.Position, GetMaxRange(unit)).ToList();
            var mapDelta = _mapKnowledge.Remove(unit, delta);
            var assetDelta = _assetKnowledge.RemoveSelf(_mapKnowledge, unit, delta, positions);
            if (mapDelta.Any())
            {
                MapKnowledgeChanged?.Invoke(this, new(_player, mapDelta));
            }
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private static int GetMaxRange(Unit unit)
        {
            return (int)Enum.GetValues<UnitDetectionBand>()
                .Select(x => unit.Type.Capabilities.GetRange(CombatCondition.None, x).GetValue()).Max();
        }
    }
}
