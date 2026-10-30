import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";

function Layout() {
    return (
        <>
            {/* shared navigation displayed on all pages using this layout */}
            <Navbar />
            <main>
                {/* The component for the current route is rendered here */}
                <Outlet />
            </main>
        </>
    );
}

export default Layout;