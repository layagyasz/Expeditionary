luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

None = MapEnvironmentTrait()
None.Key = "environment-trait-erosion-none"
None.Name = "No Erosion"
function None:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.1
	terrain.ElevationLayer.Noise.Persistence = 0.7
end

Young = MapEnvironmentTrait()
Young.Key = "environment-trait-erosion-young"
Young.Name = "Young"
function Young:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.5
	terrain.ElevationLayer.Noise.Persistence = 0.6
end

Old = MapEnvironmentTrait()
Old.Key = "environment-trait-erosion-old"
Old.Name = "Old"
function Old:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.9
	terrain.ElevationLayer.Noise.Persistence = 0.5
end


Ancient = MapEnvironmentTrait()
Ancient.Key = "environment-trait-erosion-ancient"
Ancient.Name = "Ancient"
function Ancient:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 1
	terrain.ElevationLayer.Noise.Persistence = 0.4
end

function Load()
	return { None, Young, Old, Ancient }
end