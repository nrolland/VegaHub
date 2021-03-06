﻿module VegaHub.Vega

open System
open Microsoft.AspNet.SignalR
open Newtonsoft.Json
open ImpromptuInterface.FSharp
open VegaHub

[<CLIMutable>]
type Point = { x: int; y: int }

let private sendSpec (spec: string) (hub: IHubContext) : unit =
    hub.Clients.All?parse spec 

let private toBarJSON data =
    JsonConvert.SerializeObject(data)
    |> sprintf """{
  "width": 400,
  "height": 200,
  "padding": {"top": 10, "left": 30, "bottom": 30, "right": 10},
  "data": [
    {
      "name": "table",
      "values": %s
    }
  ],
  "scales": [
    {
      "name": "x",
      "type": "ordinal",
      "range": "width",
      "domain": {"data": "table", "field": "data.x"}
    },
    {
      "name": "y",
      "range": "height",
      "nice": true,
      "domain": {"data": "table", "field": "data.y"}
    }
  ],
  "axes": [
    {"type": "x", "scale": "x"},
    {"type": "y", "scale": "y"}
  ],
  "marks": [
    {
      "type": "rect",
      "from": {"data": "table"},
      "properties": {
        "enter": {
          "x": {"scale": "x", "field": "data.x"},
          "width": {"scale": "x", "band": true, "offset": -1},
          "y": {"scale": "y", "field": "data.y"},
          "y2": {"scale": "y", "value": 0}
        },
        "update": {
          "fill": {"value": "steelblue"}
        },
        "hover": {
          "fill": {"value": "red"}
        }
      }
    }
  ]
}"""

let bar data = 
    GlobalHost.ConnectionManager.GetHubContext<WebApp.ChartHub>()
    |> sendSpec (data |> toBarJSON)
