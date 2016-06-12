#version 420

layout (location = 0) out vec4 out_color;

in vec3 tc;

layout(binding = 1) uniform sampler2D normal;
uniform vec3 lightDir;

void main(){
	out_color = vec4(0,0,0,1);
	if(dot(normalize(tc),normalize(-lightDir)) > 0.995){
		out_color = vec4(1,1,1,1);
	}
	vec2 texcoord = gl_FragCoord.xy/(textureSize(normal, 0)/2);
	if(length(texture(normal, texcoord).xyz) > 0){
		out_color = vec4(0,0,0,1);
	}
}