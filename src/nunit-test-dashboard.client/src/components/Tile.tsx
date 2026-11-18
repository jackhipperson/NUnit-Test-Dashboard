interface TileProps {
    title: string;
    number:  number;
    pct?: boolean;
}

const Tile = ({title, number, pct = false} : TileProps) => {
return (
    <div className={`rounded bg-gray-600 text-center text-white p-2 ${title == "Total" ? "col-span-2" : "col-span-1"} md:col-span-1`}>
        <p className="text-3xl">{number.toFixed(0)}{pct && "%"}</p>
        <p className="text-xs">{title}</p>
    </div>
)}

export default Tile;