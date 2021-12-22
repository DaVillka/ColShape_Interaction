using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
namespace Interaction
{
    //Клас расширения колшейпа
    public static class colshape_exceptions
    {
        //Добавляем в колшейп делегат который сработакет при нажатии клавишы
        public static void SetInteraction(this ColShape shape, Action<Player, ColShape> action)
        {
            shape.SetData<Action<Player,ColShape >> ("colShapeInteraction", action);
        }
        //Вызов делегата при нажатии клавиши
        public static void Interaction(this ColShape shape, Player player)
        {
            if (shape.HasData("colShapeInteraction")) shape.GetData<Action<Player, ColShape>>("colShapeInteraction")?.Invoke(player, shape);
        }
    }
    public static class player_exceptions
    {
        //Добавляем в персонажа колшейп на который он встал
        public static void SetPlayerColShape(this Player player, ColShape shape)
        {
            player.SetData<ColShape>("playerColShape", shape);
        }
        //Получаем записанный в персонажа колшейп
        public static ColShape GetPlayerColShape(this Player player)
        {
            if (player.HasData("playerColShape")) return player.GetData<ColShape>("playerColShape");
            else return null;
        }
    }
    //Пример реализцаии взаимодействия
    class ColShapeInteraction : Script
    {
        //Создаем колшейпы
        ColShape shape1 = NAPI.ColShape.CreateCylinderColShape(new Vector3(-425.32864, 1133.908, 325.90396), 2f, 2f, 0);
        ColShape shape2 = NAPI.ColShape.CreateCylinderColShape(new Vector3(-420.1696, 1132.7776, 325.90494), 2f, 2f, 0);
        Marker marker1 = NAPI.Marker.CreateMarker(GTANetworkAPI.MarkerType.Number1, new Vector3(-425.32864, 1133.908, 325.90396) - new Vector3(0, 0, 1f), new Vector3(), new GTANetworkAPI.Vector3(), 1f, new Color(255, 255, 255));
        Marker marker2 = NAPI.Marker.CreateMarker(GTANetworkAPI.MarkerType.Number2, new Vector3(-420.1696, 1132.7776, 325.90494) - new Vector3(0, 0, 1f), new Vector3(), new GTANetworkAPI.Vector3(), 1f, new Color(255, 255, 255));
        public ColShapeInteraction()
        {
            //присваиваем колшейпам функции для взаимодействия
            shape1.SetInteraction(playerShapeInteraction1);
            shape2.SetInteraction(playerShapeInteraction2);

        }
        //Функция второго колшейпа
        private void playerShapeInteraction2(Player player, ColShape shape)
        {
            player.SendChatMessage("Interact ColShape 2");
        }
        //Функция первого колшейпа
        private void playerShapeInteraction1( Player player, ColShape shape)
        {
            player.SendChatMessage("Interact ColShape 1");
        }
        //Ивент нажатия клавиши
        [GTANetworkAPI.RemoteEvent("pressKeyE")]
        public void OnPpressKeyE(Player player)
        {
            //Если пресонаж нажал клавишу находясь в колшейпе вызываем интерекшен колшейпа и передаем в него самого себя
            player.GetPlayerColShape()?.Interaction(player);
        }
        //Персонаж встал на колшейп
        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnPlayerEnterColshape(ColShape colShape, Player player)
        {
            //Записали колшейп в персонажа
            player.SetPlayerColShape(colShape);
        }
        //Персонаж вышел с колшейпа
        [ServerEvent(Event.PlayerExitColshape)]
        public void OnPlayerExitColshape(ColShape colShape, Player player)
        {
            //Удалили колшейп с персонажа
            player.SetPlayerColShape(null);
        }
    }
}
