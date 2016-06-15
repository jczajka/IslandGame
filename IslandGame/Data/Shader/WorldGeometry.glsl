#version 400

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

in VS_OUT{
	vec3 position;
	vec3 color;
	vec2 light;
} gs_in[3];

out GS_OUT{
	vec3 position;
	vec3 normal;
	vec3 color;
	vec2 light;
} gs_out;

void main(){
	vec3 ab = normalize(gs_in[1].position - gs_in[0].position);
    vec3 ac = normalize(gs_in[2].position - gs_in[0].position);
    vec3 normal = normalize(cross(ab, ac));
	for(int i = 0; i < 3; i++){
		gl_Position = gl_in[i].gl_Position;
		gs_out.color = gs_in[i].color;
		gs_out.position = gs_in[i].position;
		gs_out.normal = normal;
		gs_out.light = gs_in[i].light;
		EmitVertex();
	}
	EndPrimitive();
}