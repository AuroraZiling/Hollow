﻿using System.Text.Json.Nodes;
using Json.Schema;

namespace Hollow.Helpers;

public static class UigfSchemaValidator
{
    private static JsonSchema Schema { get; } = JsonSchema.FromText(SchemaJson);
    
    public static bool Validate(string json)
        => Schema.Evaluate(JsonNode.Parse(json)).IsValid;

    private const string SchemaJson =
        """
        {
            "$schema": "https://json-schema.org/draft/2020-12/schema",
            "type": "object",
            "properties": {
                "info": {
                    "type": "object",
                    "properties": {
                        "export_timestamp": {
                            "oneOf": [
                                {
                                    "type": "string"
                                },
                                {
                                    "type": "integer"
                                }
                            ],
                            "description": "导出档案的时间戳，秒级"
                        },
                        "export_app": {
                            "type": "string",
                            "description": "导出档案的 App 名称"
                        },
                        "export_app_version": {
                            "type": "string",
                            "description": "导出档案的 App 版本"
                        },
                        "version": {
                            "type": "string",
                            "pattern": "^v\\d+\\.\\d+$",
                            "description": "导出档案的 UIGF 版本号，格式为 'v{major}.{minor}'，如 v4.0"
                        }
                    },
                    "required": [
                        "export_timestamp",
                        "export_app",
                        "export_app_version",
                        "version"
                    ]
                },
                "hk4e": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "uid": {
                                "oneOf": [
                                    {
                                        "type": "string"
                                    },
                                    {
                                        "type": "integer"
                                    }
                                ],
                                "description": "UID"
                            },
                            "timezone": {
                                "type": "integer",
                                "description": "时区偏移，由米哈游 API 返回，若与服务器时区不同请注意 list 中 time 的转换"
                            },
                            "lang": {
                                "type": "string",
                                "description": "语言代码",
                                "enum": [
                                    "de-de",
                                    "en-us",
                                    "es-es",
                                    "fr-fr",
                                    "id-id",
                                    "it-it",
                                    "ja-jp",
                                    "ko-kr",
                                    "pt-pt",
                                    "ru-ru",
                                    "th-th",
                                    "tr-tr",
                                    "vi-vn",
                                    "zh-cn",
                                    "zh-tw"
                                ]
                            },
                            "list": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "uigf_gacha_type": {
                                            "type": "string",
                                            "description": "UIGF 卡池类型，用于区分卡池类型不同，但卡池保底计算相同的物品",
                                            "enum": [
                                                "100",
                                                "200",
                                                "301",
                                                "302",
                                                "500"
                                            ]
                                        },
                                        "gacha_type": {
                                            "type": "string",
                                            "description": "卡池类型，米哈游 API 返回",
                                            "enum": [
                                                "100",
                                                "200",
                                                "301",
                                                "302",
                                                "400",
                                                "500"
                                            ]
                                        },
                                        "item_id": {
                                            "type": "string",
                                            "description": "物品的内部 ID"
                                        },
                                        "count": {
                                            "type": "string",
                                            "description": "物品个数，一般为1，米哈游 API 返回"
                                        },
                                        "time": {
                                            "type": "string",
                                            "description": "抽取物品时对应时区（timezone）下的当地时间"
                                        },
                                        "name": {
                                            "type": "string",
                                            "description": "物品名称，米哈游 API 返回"
                                        },
                                        "item_type": {
                                            "type": "string",
                                            "description": "物品类型，米哈游 API 返回"
                                        },
                                        "rank_type": {
                                            "type": "string",
                                            "description": "物品等级，米哈游 API 返回"
                                        },
                                        "id": {
                                            "type": "string",
                                            "description": "记录内部 ID，米哈游 API 返回"
                                        }
                                    },
                                    "required": [
                                        "uigf_gacha_type",
                                        "gacha_type",
                                        "item_id",
                                        "time",
                                        "id"
                                    ]
                                }
                            }
                        },
                        "required": [
                            "uid",
                            "timezone",
                            "list"
                        ]
                    }
                },
                "hkrpg": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "uid": {
                                "oneOf": [
                                    {
                                        "type": "string"
                                    },
                                    {
                                        "type": "integer"
                                    }
                                ],
                                "description": "UID"
                            },
                            "timezone": {
                                "type": "integer",
                                "description": "时区偏移，由米哈游 API 返回，若与服务器时区不同请注意 list 中 time 的转换"
                            },
                            "lang": {
                                "type": "string",
                                "description": "语言代码",
                                "enum": [
                                    "de-de",
                                    "en-us",
                                    "es-es",
                                    "fr-fr",
                                    "id-id",
                                    "it-it",
                                    "ja-jp",
                                    "ko-kr",
                                    "pt-pt",
                                    "ru-ru",
                                    "th-th",
                                    "tr-tr",
                                    "vi-vn",
                                    "zh-cn",
                                    "zh-tw"
                                ]
                            },
                            "list": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "gacha_id": {
                                            "type": "string",
                                            "description": "卡池 Id"
                                        },
                                        "gacha_type": {
                                            "type": "string",
                                            "description": "卡池类型",
                                            "enum": [
                                                "1",
                                                "2",
                                                "11",
                                                "12"
                                            ]
                                        },
                                        "item_id": {
                                            "type": "string",
                                            "description": "物品的内部 ID"
                                        },
                                        "count": {
                                            "type": "string",
                                            "description": "物品个数，一般为1，米哈游 API 返回"
                                        },
                                        "time": {
                                            "type": "string",
                                            "description": "抽取物品时对应时区（timezone）下的当地时间"
                                        },
                                        "name": {
                                            "type": "string",
                                            "description": "物品名称，米哈游 API 返回"
                                        },
                                        "item_type": {
                                            "type": "string",
                                            "description": "物品类型，米哈游 API 返回"
                                        },
                                        "rank_type": {
                                            "type": "string",
                                            "description": "物品等级，米哈游 API 返回"
                                        },
                                        "id": {
                                            "type": "string",
                                            "description": "记录内部 ID，米哈游 API 返回"
                                        }
                                    },
                                    "required": [
                                        "gacha_type",
                                        "gacha_id",
                                        "time",
                                        "item_id",
                                        "id"
                                    ]
                                }
                            }
                        },
                        "required": [
                            "uid",
                            "timezone",
                            "list"
                        ]
                    }
                },
                "nap": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "uid": {
                                "oneOf": [
                                    {
                                        "type": "string"
                                    },
                                    {
                                        "type": "integer"
                                    }
                                ],
                                "description": "UID"
                            },
                            "timezone": {
                                "type": "integer",
                                "description": "时区偏移，由米哈游 API 返回，若与服务器时区不同请注意 list 中 time 的转换"
                            },
                            "lang": {
                                "type": "string",
                                "description": "语言代码",
                                "enum": [
                                    "de-de",
                                    "en-us",
                                    "es-es",
                                    "fr-fr",
                                    "id-id",
                                    "it-it",
                                    "ja-jp",
                                    "ko-kr",
                                    "pt-pt",
                                    "ru-ru",
                                    "th-th",
                                    "tr-tr",
                                    "vi-vn",
                                    "zh-cn",
                                    "zh-tw"
                                ]
                            },
                            "list": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "gacha_id": {
                                            "type": "string",
                                            "description": "卡池 Id"
                                        },
                                        "gacha_type": {
                                            "type": "string",
                                            "description": "卡池类型",
                                            "enum": [
                                                "1",
                                                "2",
                                                "3",
                                                "5"
                                            ]
                                        },
                                        "item_id": {
                                            "type": "string",
                                            "description": "物品的内部 ID"
                                        },
                                        "count": {
                                            "type": "string",
                                            "description": "物品个数，一般为1，米哈游 API 返回"
                                        },
                                        "time": {
                                            "type": "string",
                                            "description": "抽取物品时对应时区（timezone）下的当地时间"
                                        },
                                        "name": {
                                            "type": "string",
                                            "description": "物品名称，米哈游 API 返回"
                                        },
                                        "item_type": {
                                            "type": "string",
                                            "description": "物品类型，米哈游 API 返回"
                                        },
                                        "rank_type": {
                                            "type": "string",
                                            "description": "物品等级，米哈游 API 返回"
                                        },
                                        "id": {
                                            "type": "string",
                                            "description": "记录内部 ID，米哈游 API 返回"
                                        }
                                    },
                                    "required": [
                                        "gacha_type",
                                        "item_id",
                                        "time",
                                        "id"
                                    ]
                                }
                            }
                        },
                        "required": [
                            "uid",
                            "timezone",
                            "list"
                        ]
                    }
                }
            },
            "required": [
                "info"
            ]
        }
        """;
}