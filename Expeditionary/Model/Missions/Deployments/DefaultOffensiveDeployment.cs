﻿using Expeditionary.Evaluation;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Model.Formations;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;

namespace Expeditionary.Model.Missions.Deployments
{
    public record class DefaultOffensiveDeployment(MapDirection Direction) : IDeployment
    {
        public void Setup(FormationTemplate formation, Player player, Match match, PlayerSetupContext context)
        {
            var map = match.GetMap();
            var region = new EdgeMapRegion(Direction);
            var origin = 
                region.Range(match.GetMap())
                    .MaxBy(x => TileConsiderations.Evaluate(
                        TileConsiderations.Combine(
                            TileConsiderations.Weight(0.1f, TileConsiderations.Noise(context.Parent.Random)), 
                            TileConsiderations.Roading(map)), 
                        x,
                        map));
            var exemplar = 
                formation.GetUnitTypesAndRoles()
                    .GroupBy(x => x.UnitType)
                    .ToDictionary(x => x.Key, x => x.Count()).MaxBy(x => x.Value).Key;
            int distance = match.GetMap().GetAxisSize(Direction) / 3;
            var extent = Pathing.GetPathField(map, origin, exemplar.Movement, distance);
            var sdf = SignedDistanceField.FromPathField(extent, distance >> 1);
            DeploymentHelper.DeployInRegion(
                formation, 
                player, 
                match, 
                sdf,
                new ExplicitMapRegion(extent.Select(x => x.Destination)),
                MapDirectionUtils.Invert(Direction),
                TileConsiderations.Combine(
                    TileConsiderations.Essential(TileConsiderations.Land),
                    TileConsiderations.Direction(origin, MapDirectionUtils.Invert(Direction)),
                    TileConsiderations.Forestation,
                    TileConsiderations.Urbanization,
                    TileConsiderations.Roading(match.GetMap()),
                    TileConsiderations.Weight(0.1f, TileConsiderations.Noise(context.Parent.Random))),
                context);
        }
    }
}
