#version 420

layout (location = 0) out vec4 out_color;

in vec3 tc;

vec3 toGamma(vec3 color);

void main(){
	out_color = vec4(toGamma(mix(vec3(1,0.8,0.6),vec3(0,0.8,1) * 0.7, clamp(((normalize(tc).y*3+1)/2),0,1))),1);
}