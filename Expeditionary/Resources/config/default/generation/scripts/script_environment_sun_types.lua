luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

ClassO = MapEnvironmentModifier()
ClassO.Key = "environment-modifier-star-class-o"
ClassO.Name = "Class O Star"
function ClassO:Apply(environment)
	 environment.Appearance.LightTemperature = 37500
end

ClassB = MapEnvironmentModifier()
ClassB.Key = "environment-modifier-star-class-b"
ClassB.Name = "Class B Star"
function ClassB:Apply(environment)
	 environment.Appearance.LightTemperature = 20000
end

ClassA = MapEnvironmentModifier()
ClassA.Key = "environment-modifier-star-class-a"
ClassA.Name = "Class A Star"
function ClassA:Apply(environment)
	 environment.Appearance.LightTemperature = 8750
end

ClassF = MapEnvironmentModifier()
ClassF.Key = "environment-modifier-star-class-f"
ClassF.Name = "Class F Star"
function ClassF:Apply(environment)
	 environment.Appearance.LightTemperature = 6750
end

ClassG = MapEnvironmentModifier()
ClassG.Key = "environment-modifier-star-class-g"
ClassG.Name = "Class G Star"
function ClassG:Apply(environment)
	 environment.Appearance.LightTemperature = 5600
end

ClassK = MapEnvironmentModifier()
ClassK.Key = "environment-modifier-star-class-k"
ClassK.Name = "Class K Star"
function ClassK:Apply(environment)
	 environment.Appearance.LightTemperature = 4450
end

ClassM = MapEnvironmentModifier()
ClassM.Key = "environment-modifier-star-class-m"
ClassM.Name = "Class M Star"
function ClassM:Apply(environment)
	 environment.Appearance.LightTemperature = 3050
end

BlueGiant = MapEnvironmentModifier()
BlueGiant.Key = "environment-modifier-star-blue-giant"
BlueGiant.Name = "Blue Giant Star"
function BlueGiant:Apply(environment)
	 environment.Appearance.LightTemperature = 31000
end

RedGiant = MapEnvironmentModifier()
RedGiant.Key = "environment-modifier-star-red-giant"
RedGiant.Name = "Red Giant Star"
function RedGiant:Apply(environment)
	 environment.Appearance.LightTemperature = 3700
end

BrownDwarf = MapEnvironmentModifier()
BrownDwarf.Key = "environment-modifier-star-brown-dwarf"
BrownDwarf.Name = "Brown Dwarf Star"
function BrownDwarf:Apply(environment)
	 environment.Appearance.LightTemperature = 1500
end

WhiteDwarf = MapEnvironmentModifier()
WhiteDwarf.Key = "environment-modifier-star-white-dwarf"
WhiteDwarf.Name = "White Dwarf Star"
function WhiteDwarf:Apply(environment)
	 environment.Appearance.LightTemperature = 24000
end

Sol = MapEnvironmentModifier()
Sol.Key = "environment-modifier-star-sol"
Sol.Name = "Sol"
function Sol:Apply(environment)
	 environment.Appearance.LightTemperature = 5778
end

function Load()
	return { ClassO, ClassB, ClassA, ClassF, ClassG, ClassK, ClassM, BlueGiant, RedGiant, BrownDwarf, WhiteDwarf, Sol }
end