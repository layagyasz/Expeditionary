using Cardamom.Collections;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Matches.Assets;
using Expeditionary.Model.Matches.Combat;
using Expeditionary.Model.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Knowledge
{
    public class LimitedPlayerKnowledge : IPlayerKnowledge
    {
        public EventHandler<AssetKnowledgeChangedEventArgs>? AssetKnowledgeChanged { get; set; }
        public EventHandler<MapKnowledgeChangedEventArgs>? MapKnowledgeChanged { get; set; }

        private readonly MatchPlayer _player;
        private readonly Map _map;
        private readonly LimitedAssetKnowledge _assetKnowledge;
        private readonly LimitedMapKnowledge _mapKnowledge;

        public LimitedPlayerKnowledge(
            MatchPlayer player, Map map, LimitedAssetKnowledge assetKnowledge, LimitedMapKnowledge mapKnowledge)
        {
            _player = player;
            _map = map;
            _assetKnowledge = assetKnowledge;
            _mapKnowledge = mapKnowledge;
        }
        
        public SingleAssetKnowledge GetAsset(IMatchAsset asset)
        {
            return _assetKnowledge.Get(asset);
        }

        public SingleTileKnowledge GetTile(Vector3i hex)
        {
            return _mapKnowledge.Get(hex);
        }

        public void Destroy(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions)
        {
            if (asset is MatchUnit unit && unit.Player == _player)
            {
                DestroySelf(unit, positions);
            }
            else
            {
                DestroyOther(asset);
            }
        }

        public void Move(IMatchAsset asset, Pathing.Path path, MultiMap<Vector3i, IMatchAsset> positions)
        {
            if (asset is MatchUnit unit && unit.Player == _player)
            {
                MoveSelf(unit, path, positions);
            }
            else
            {
                MoveOther(asset, path);
            }
        }

        public void Place(IMatchAsset asset, Vector3i position, MultiMap<Vector3i, IMatchAsset> positions)
        {
            if (asset is MatchUnit unit && unit.Player == _player)
            {
                PlaceSelf(unit, position, positions);
            }
            else
            {
                PlaceOther(asset, position);
            }
        }

        public void Remove(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions)
        {
            if (asset is MatchUnit unit && unit.Player == _player)
            {
                RemoveSelf(unit, positions);
            }
            else
            {
                RemoveOther(asset);
            }
        }

        public void Suppress(IMatchAsset asset, MultiMap<Vector3i, IMatchAsset> positions)
        {
            if (asset is MatchUnit unit && unit.Player == _player)
            {
                RemoveSelf(unit, positions);
            }
        }

        private void DestroyOther(IMatchAsset asset)
        {
            var assetDelta = _assetKnowledge.DestroyOther(asset);
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void DestroySelf(MatchUnit unit, MultiMap<Vector3i, IMatchAsset> positions)
        {
            var delta = Sighting.GetSightField(_map, unit.Position, GetMaxRange(unit)).ToList();
            var mapDelta = _mapKnowledge.Remove(unit, delta);
            var assetDelta = _assetKnowledge.DestroySelf(_mapKnowledge, unit, delta, positions);
            if (mapDelta.Any())
            {
                MapKnowledgeChanged?.Invoke(this, new(_player, mapDelta));
            }
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void MoveOther(IMatchAsset asset, Pathing.Path path)
        {
            var assetDelta = _assetKnowledge.MoveOther(_mapKnowledge, asset, path);
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void MoveSelf(MatchUnit unit, Pathing.Path path, MultiMap<Vector3i, IMatchAsset> positions)
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

        private void PlaceOther(IMatchAsset asset, Vector3i position)
        {
            var assetDelta = _assetKnowledge.AddOther(_mapKnowledge, asset, position);
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void PlaceSelf(MatchUnit unit, Vector3i position, MultiMap<Vector3i, IMatchAsset> positions)
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

        private void RemoveOther(IMatchAsset asset)
        {
            var assetDelta = _assetKnowledge.RemoveOther(asset);
            if (assetDelta.Any())
            {
                AssetKnowledgeChanged?.Invoke(this, new(_player, assetDelta));
            }
        }

        private void RemoveSelf(MatchUnit unit, MultiMap<Vector3i, IMatchAsset> positions)
        {
            if (!unit.IsActive)
            {
                return;
            }
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

        private static int GetMaxRange(MatchUnit unit)
        {
            return (int)Enum.GetValues<UnitDetectionBand>()
                .Select(x => unit.Type.Capabilities.GetRange(CombatCondition.None, x).GetValue()).Max();
        }
    }
}
