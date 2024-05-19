/**
 * SPDX-FileCopyrightText: 2024 Ng Jun Xiang <contact@ngjx.org>
 * SPDX-License-Identifier: GPL-3.0-only
 *
 * Name: Ng Jun Xiang
 * Admin: 230725N
 */

namespace MushroomPocket.Core.DungeonGameLogic;


public static class ASCIIArt
{
    /// Reference: https://ascii.co.uk/art/dragon
    public static readonly string DungeonMaster = String.Join(
        "\n",
        @"    .    .                          ",
        @"    \\  //  .''-.     .-.           ",
        @"    \ \/ /.'     '-.-'   '.         ",
        @"~__\(    )/ __~            '.    ..~",
        @"(  . \!!/    . )     .-''-.  '..~~~~",
        @" \ | (--)---| /'-..-'BP    '-..-~'  ",
        @"  ^^^ ''   ^^^                      "
    );
    public static readonly Dimension DungeonMasterDimensions = new Dimension(36, 6);

    /// Reference chatgpt.com
    public static readonly string Character = String.Join(
        "\n",
        @" (\_/)",
        @"( •_•)",
        @"/ >🌸 "
    );
    public static readonly Dimension CharacterDimensions = new Dimension(5, 3);
}
