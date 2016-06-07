// shadertype=glsl
#version 420

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedo;
//layout (location = 3) out float gGodrays;

in GS_OUT{
	vec3 position;
	vec3 normal;
	vec3 color;
	vec2 light;
} fs_in;

void main(){
	
	gPosition = fs_in.position;
	gNormal = normalize(fs_in.normal);

	//vec3 color = vec3(1,1,1);//mix(vec3(0.2,0.4,0.8), vec3(0.2,0.6,0.3), clamp(fs_in.position.y+1f,0.0,1.0));

	gAlbedo = vec4(fs_in.color, 0.1f);
	//gGodrays = 0.0f;
	
}