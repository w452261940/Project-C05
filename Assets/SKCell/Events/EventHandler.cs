﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SKCell
{
    /// <summary>
    /// Container of events. Handle single-section event management.
    /// </summary>
    public class EventHandler
    {
        public int uid = -1;

        public Dictionary<int, SJEvent> dict = new Dictionary<int, SJEvent>();

        public EventHandler(int uid)
        {
            this.uid = uid;
        }

        /// <summary>
        /// Dispatch an event. If not registered, will register as new.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public bool DispatchEvent(int id)
        {
            if (dict.ContainsKey(id))
            {
                if(dict[id].action!=null)
                    dict[id].action.Invoke();
                return true;
            }
            else
            {
                RegisterEvent(new SJEvent(), id);
                //CommonUtils.EditorLogNormal($"EventHandler.DispatchEvent() --- target event id not found. Auto registered.");
                return false;
            }
        }

        /// <summary>
        /// Add listener to an event. If not registered, will register as new.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddListener(int id, SJEvent t_event)
        {
            if (dict.ContainsKey(id))
            {
                dict[id].action += t_event.action;
                return true;
            }
            else
            {
                RegisterEvent(new SJEvent(), id);
                dict[id].action += t_event.action;
                //CommonUtils.EditorLogNormal($"EventHandler.AddListener() --- target event id not found. Auto registered.");
                return false;
            }
        }

        /// <summary>
        /// Remove listener to an event. If not registered, will register as new.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveListener(int id, SJEvent t_event)
        {
            if (dict.ContainsKey(id))
            {
                dict[id].action -= t_event.action;
                return true;
            }
            else
            {
                CommonUtils.EditorLogWarning($"EventHandler.RemoveListener() --- target event id not found.");
                return false;
            }
        }

        /// <summary>
        /// Register an event with a unique id. Will override if there are duplicate ids.
        /// </summary>
        /// <param name="t_event"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RegisterEvent(SJEvent t_event, int id)
        {
            return CommonUtils.InsertOrUpdateKeyValueInDictionary(dict, id, t_event);
        }

        /// <summary>
        /// Remove a registered event with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public bool RemoveEvent(int id)
        {
            return CommonUtils.RemoveKeyInDictionary(dict, id);
        }
    }
}
