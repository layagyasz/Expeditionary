luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

None = MapEnvironmentModifier()
None.Key = "environment-modifier-erosion-none"
None.Name = "No Erosion"
function None:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.1
end

Young = MapEnvironmentModifier()
Young.Key = "environment-modifier-erosion-young"
Young.Name = "Young"
function Young:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.5
end

Old = MapEnvironmentModifier()
Old.Key = "environment-modifier-erosion-old"
Old.Name = "Old"
function Old:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 0.9
end


Ancient = MapEnvironmentModifier()
Ancient.Key = "environment-modifier-erosion-ancient"
Ancient.Name = "Ancient"
function Ancient:Apply(environment)
	local terrain = environment.Parameters.Terrain
	terrain.SoilCover = 1
end

function Load()
	return { None, Young, Old, Ancient }
end