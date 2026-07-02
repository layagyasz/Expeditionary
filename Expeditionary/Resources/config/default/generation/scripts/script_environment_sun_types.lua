luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

ClassO = MapEnvironmentTrait()
ClassO.Key = "environment-trait-star-class-o"
function ClassO:Apply(environment)
	 environment.Appearance.LightTemperature = 37500
end

ClassB = MapEnvironmentTrait()
ClassB.Key = "environment-trait-star-class-b"
function ClassB:Apply(environment)
	 environment.Appearance.LightTemperature = 20000
end

ClassA = MapEnvironmentTrait()
ClassA.Key = "environment-trait-star-class-a"
function ClassA:Apply(environment)
	 environment.Appearance.LightTemperature = 8750
end

ClassF = MapEnvironmentTrait()
ClassF.Key = "environment-trait-star-class-f"
function ClassF:Apply(environment)
	 environment.Appearance.LightTemperature = 6750
end

ClassG = MapEnvironmentTrait()
ClassG.Key = "environment-trait-star-class-g"
function ClassG:Apply(environment)
	 environment.Appearance.LightTemperature = 5600
end

ClassK = MapEnvironmentTrait()
ClassK.Key = "environment-trait-star-class-k"
function ClassK:Apply(environment)
	 environment.Appearance.LightTemperature = 4450
end

ClassM = MapEnvironmentTrait()
ClassM.Key = "environment-trait-star-class-m"
function ClassM:Apply(environment)
	 environment.Appearance.LightTemperature = 3050
end

BlueGiant = MapEnvironmentTrait()
BlueGiant.Key = "environment-trait-star-blue-giant"
function BlueGiant:Apply(environment)
	 environment.Appearance.LightTemperature = 31000
end

RedGiant = MapEnvironmentTrait()
RedGiant.Key = "environment-trait-star-red-giant"
function RedGiant:Apply(environment)
	 environment.Appearance.LightTemperature = 3700
end

BrownDwarf = MapEnvironmentTrait()
BrownDwarf.Key = "environment-trait-star-brown-dwarf"
function BrownDwarf:Apply(environment)
	 environment.Appearance.LightTemperature = 1500
end

WhiteDwarf = MapEnvironmentTrait()
WhiteDwarf.Key = "environment-trait-star-white-dwarf"
function WhiteDwarf:Apply(environment)
	 environment.Appearance.LightTemperature = 24000
end

Sol = MapEnvironmentTrait()
Sol.Key = "environment-trait-star-sol"
function Sol:Apply(environment)
	 environment.Appearance.LightTemperature = 5778
end

function Load()
	return { ClassO, ClassB, ClassA, ClassF, ClassG, ClassK, ClassM, BlueGiant, RedGiant, BrownDwarf, WhiteDwarf, Sol }
end