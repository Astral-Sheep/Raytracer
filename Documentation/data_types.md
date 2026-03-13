# Raytracer shader data types

## Shape (2 texels)
|       Field      | Type | Size |
|:----------------:|:----:|:----:|
|*type*            |`int` |4     |
|*data_texel_index*|`int` |4     |
|*bound_min*       |`vec3`|12    |
|*bound_max*       |`vec3`|12    |

## Bounding Volume (1 texel)
|    Field    | Type | Size |
|:-----------:|:----:|:----:|
|*child_start*|`int` |4    |
|*child_count*|`int` |4    |
|*padding*    |`∅`   |8    |

## Sphere (2 texels)
|     Field      | Type  | Size |
|:--------------:|:-----:|:----:|
|*center*        |`vec3` |12    |
|*radius*        |`float`|4     |
|*material_index*|`int`  |4     |
|*padding*       |`∅`    |12    |

## Mesh (5 texels)
|      Field     | Type | Size |
|:--------------:|:----:|:----:|
|*tri_start*     |`int` |4     |
|*tri_count*     |`int` |4     |
|*transform*     |`mat4`|64    |
|*material_index*|`int` |4     |
|*padding*       |`∅`   |4     |

## MeshVertex (2 texels)
|  Field   | Type | Size |
|:--------:|:----:|:----:|
|*position*|`vec3`|12    |
|*normal*  |`vec3`|12    |
|*uv*      |`vec2`|8     |

## MeshTriangle (0.75 texel)
|  Field   | Type | Size |
|:--------:|:----:|:----:|
|*v0*      |`int` |4     |
|*v1*      |`int` |4     |
|*v2*      |`int` |4     |

## Material (1 texel)
|         Field        | Type  | Size |
|:--------------------:|:-----:|:----:|
|*type*                |`byte` |1     |
|*texture_index*       |`short`|2     |
|*color*               |`byte3`|3     |
|*emissive*            |`byte3`|3     |
|*emissive_intensity*  |`half` |2     |
|*smoothness*          |`byte` |1     |
|*specular_color*      |`byte3`|3     |
|*specular_probability*|`byte` |1     |

