# Raytracer shader data types

## Shape (0.75 texel)
|      Field     | Type | Size |
|:--------------:|:----:|:----:|
|*type*          |`int` |4     |
|*data_index*    |`int` |4     |
|*material_index*|`int` |4     |

## Bounding Volume (2 texels)
|      Field     | Type | Size |
|:--------------:|:----:|:----:|
|*bound_min*     |`vec3`|12    |
|*bound_max*     |`vec3`|12    |
|*child0*        |`int` |4     |
|*child1*        |`int` |4     |

## Sphere (1 texel)
| Field  | Type  | Size |
|:------:|:-----:|:----:|
|*center*|`vec3` |12    |
|*radius*|`float`|4     |

## Mesh (6 texels)
|      Field     | Type | Size |
|:--------------:|:----:|:----:|
|*tri_start*     |`int` |4     |
|*tri_count*     |`int` |4     |
|*bound_min*     |`vec3`|12    |
|*bound_max*     |`vec3`|12    |
|*transform*     |`mat4`|64    |

## Vertex (2 texels)
|  Field   | Type | Size |
|:--------:|:----:|:----:|
|*position*|`vec3`|12    |
|*normal*  |`vec3`|12    |
|*uv*      |`vec2`|8     |

## Triangle (0.75 texel)
|  Field   | Type | Size |
|:--------:|:----:|:----:|
|*v0_index*|`int` |4     |
|*v1_index*|`int` |4     |
|*v2_index*|`int` |4     |

## Material (4 texels with 4 bytes padding)
|         Field        | Type  | Size |
|:--------------------:|:-----:|:----:|
|*type*                |`int`  |4     |
|*color*               |`vec4` |16    |
|*emissive*            |`vec3` |12    |
|*emissive_intensity*  |`float`|4     |
|*smoothness*          |`float`|4     |
|*specular_color*      |`vec3` |12    |
|*specular_probability*|`float`|4     |
|*texture_index*       |`int`  |4     |

