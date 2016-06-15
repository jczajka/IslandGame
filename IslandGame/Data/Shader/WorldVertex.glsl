// shadertype=glsl
#version 400

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 color;
layout(location = 2) in vec2 light;

layout(location = 10) in mat4 model;

out VS_OUT{
	vec3 position;
	vec3 color;
	vec2 light;
	vec2 velocity;
} vs_out;

uniform mat4 mvp;
uniform mat4 premvp;

vec3 toGamma(vec3 color);

void main(){
	vec4 transformedpos = model * vec4(position , 1);
	vs_out.position = transformedpos.xyz / transformedpos.w;
	vs_out.color = toGamma(color);
	vs_out.light = light;

	gl_Position = mvp * transformedpos;

	vec4 oldpos = premvp * transformedpos;
	vs_out.velocity = (gl_Position.xy / gl_Position.w) - (oldpos.xy / oldpos.w); 
}