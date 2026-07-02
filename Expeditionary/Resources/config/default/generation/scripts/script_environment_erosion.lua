luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

None = MapEnvironmentTrait()
None.Key = "environment-trait-erosion-none"
function None:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.1
	terrain.ElevationLayer.Noise.Persistence = 0.7
end

Young = MapEnvironmentTrait()
Young.Key = "environment-trait-erosion-young"
function Young:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.5
	terrain.ElevationLayer.Noise.Persistence = 0.6
end

Old = MapEnvironmentTrait()
Old.Key = "environment-trait-erosion-old"
function Old:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.9
	terrain.ElevationLayer.Noise.Persistence = 0.5
end


Ancient = MapEnvironmentTrait()
Ancient.Key = "environment-trait-erosion-ancient"
function Ancient:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 1
	terrain.ElevationLayer.Noise.Persistence = 0.4
end

function Load()
	return { None, Young, Old, Ancient }
end