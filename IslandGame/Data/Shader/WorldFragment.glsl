// shadertype=glsl
#version 420

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec3 gAlbedo;
layout (location = 3) out vec3 gLight;

in GS_OUT{
	vec3 position;
	vec3 normal;
	vec3 color;
	vec2 light;
} fs_in;

void main(){
	
	gPosition = fs_in.position;
	gNormal = normalize(fs_in.normal);
	gAlbedo = fs_in.color;
	gLight = vec3(64.0/256.0, 0.2, 0);//fs_in.light
	
}