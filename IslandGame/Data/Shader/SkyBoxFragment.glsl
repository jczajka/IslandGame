// shadertype=glsl
#version 420

layout (location = 0) out vec4 out_color;
layout (location = 1) out vec4 bright_out_color;

in vec3 tc;

uniform vec3 lightDir;

vec3 toGamma(vec3 color);

void main(){
	out_color = vec4(toGamma(mix(vec3(1,0.8,0.6),vec3(0,0.8,1) * 0.7, clamp(((normalize(tc).y*3+1)/2),0,1))),1);
	bright_out_color = vec4(0,0,0,1);
	//if(dot(normalize(tc),normalize(-lightDir)) > 0.995){
	//	out_color = vec4(toGamma(vec3(1,0.8,0.3)),1);
	//}
	//gGodrays = 0.0f;
	//if(dot(normalize(tc),lightDir) > 0.995){
	//	gGodrays = 1.0f;
	//}
}